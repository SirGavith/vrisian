using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace vrisian
{
    public class Animation
    {
        public ByteImage SourceImage { get; set; }
        public List<AnimationFrame> Frames { get; set; } = new List<AnimationFrame> { };
        public int CurrentSpriteIndex { get; set; }
        public bool IsSet = false;

        public bool Interpolate { get; set; }
        public int FrameTime { get; set; }
        public string MCMetaPath {get; set;}
        public bool MCMetaExists => File.Exists(MCMetaPath);

        public XY SpriteSize => new XY(SourceImage.Width, SourceImage.Width * SpriteSizeRatio.Y / SpriteSizeRatio.X);
        public int SpriteCount { get; private set; }

        public XY SpriteSizeRatio { get; set; }

        public XY GetSpriteOffset(int F) => new XY(0, SpriteSize.Y * F);
        public ByteImage GetSprite(int F) => new ByteImage(Pixel.Slice(SourceImage.Source, GetSpriteOffset(F), SpriteSize));

        public XY CurrentSpriteOffset => GetSpriteOffset(CurrentSpriteIndex);
        public ByteImage CurrentSprite => GetSprite(CurrentSpriteIndex);

        public Animation(ByteImage img)
        {
            SourceImage = img;
        }
        public Animation(ByteImage img, AnimationMCMeta mcmeta)
        {
            ParseMCMeta(img, mcmeta);
        }
        public Animation(ByteImage img, DirectoryItem file)
        {
            MCMetaPath = file.FullPath + ".mcmeta";
            if (!MCMetaExists) { return; }

            string jsonText = File.ReadAllText(file.FullPath + ".mcmeta");
            if (string.IsNullOrEmpty(jsonText)) { return; }
            ParseMCMeta(img, JsonSerializer.Deserialize<AnimationFile>(jsonText).Animation);
        }

        private void ParseMCMeta(ByteImage img, AnimationMCMeta mcmeta)
        {
            SourceImage = img;
            Interpolate = mcmeta.Interpolate;
            FrameTime = mcmeta.FrameTime;
            Frames = new List<AnimationFrame>();
            SpriteSizeRatio = new XY(mcmeta.Width, mcmeta.Height);


            SpriteCount = img.Height / SpriteSize.Y;

            if (!Utils.IsInteger(img.Height / SpriteSize.Y))
            {
                throw new Exception("The width and height in the mcmeta do not match up with the size of the image");
            }


            if (mcmeta.JsonFrames == null)
            {
                for (int i = 0; i < SpriteCount; i++)
                {
                    Frames.Add(new AnimationFrame() { Index = i, Time = FrameTime });
                }
            }
            else
            {
                JsonElement JsonFrames = (JsonElement)mcmeta.JsonFrames;

                foreach (var f in JsonFrames.EnumerateArray())
                {
                    if (f.ValueKind == JsonValueKind.Number)
                    {
                        Frames.Add(new AnimationFrame() { Index = f.GetInt32(), Time = FrameTime });
                    }
                    else if (f.ValueKind == JsonValueKind.Object)
                    {
                        Frames.Add(new AnimationFrame() { Index = f.GetProperty("index").GetInt32(), Time = f.GetProperty("time").GetInt32() });
                    }
                }
            }
            IsSet = true;
        }

        public void SaveMCMeta(string _MCMetaPath = null)
        {

            _MCMetaPath = _MCMetaPath ?? MCMetaPath ?? throw new ArgumentNullException("MCMetaPath must be specified to save the MCMeta");

            AnimationMCMetaWrite MCMeta = new AnimationMCMetaWrite()
            {
                FrameTime = FrameTime,
                Interpolate = Interpolate,
                Width = SpriteSizeRatio.X,
                Height = SpriteSizeRatio.Y,
                Frames = Frames
            };
            string jsonString = JsonSerializer.Serialize(new AnimationFileWrite() { Animation = MCMeta }, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(_MCMetaPath, jsonString);
        }

        public void AddNewFrame()
        {
            SourceImage.Reshape(new XY(SourceImage.Width, SourceImage.Height + SpriteSize.Y));
            Frames.Add(new AnimationFrame() { Index = Frames.Count, Time = FrameTime });
        }

        public void PreviousFrame()
        {
            CurrentSpriteIndex--;
            if (CurrentSpriteIndex < 0) { CurrentSpriteIndex = Frames.Count - 1; }
        }

        public void NextFrame()
        {
            CurrentSpriteIndex++;
            CurrentSpriteIndex %= Frames.Count;
        }

        public Animation Copy()
        {
            return new Animation(SourceImage.Copy())
            {
                Frames = Frames.ToArray().ToList(),
                CurrentSpriteIndex = CurrentSpriteIndex,
                Interpolate = Interpolate,
                FrameTime = FrameTime,
                MCMetaPath = MCMetaPath,
                SpriteCount = SpriteCount,
                SpriteSizeRatio = new XY(SpriteSizeRatio.X, SpriteSizeRatio.Y),
                IsSet = true
            };
        }
    }

    public class AnimationFrame
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }
        [JsonPropertyName("time")]
        public int Time { get; set; }
    }

    struct AnimationFile
    {
        [JsonPropertyName("animation")]
        public AnimationMCMeta Animation { get; set; }
    }

    public class AnimationMCMeta
    {
        [JsonPropertyName("interpolate")]
        public bool Interpolate { get; set; } = false;
        [JsonPropertyName("width")]
        public int Width { get; set; } = 1;
        [JsonPropertyName("height")]
        public int Height { get; set; } = 1;
        [JsonPropertyName("frametime")]
        public int FrameTime { get; set; } = 1;
        [JsonPropertyName("frames")]
        public JsonElement? JsonFrames { get; set; }

    }

    struct AnimationFileWrite
    {
        [JsonPropertyName("animation")]
        public AnimationMCMetaWrite Animation { get; set; }
    }

    struct AnimationMCMetaWrite
    {
        [JsonPropertyName("interpolate")]
        public bool Interpolate { get; set; }
        [JsonPropertyName("width")]
        public int Width { get; set; }
        [JsonPropertyName("height")]
        public int Height { get; set; }
        [JsonPropertyName("frametime")]
        public int FrameTime { get; set; }
        [JsonPropertyName("frames")]
        public List<AnimationFrame> Frames { get; set; }
    }
}
