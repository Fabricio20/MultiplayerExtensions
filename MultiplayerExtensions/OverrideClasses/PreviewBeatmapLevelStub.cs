﻿using BeatSaverSharp;
using MultiplayerExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace MultiplayerExtensions.OverrideClasses
{
    public class PreviewBeatmapLevelStub : IPreviewBeatmapLevel
    {
        private object _getlock = new object();
        private Task<Beatmap> _getBeatmap;
        public Task<Beatmap> GetBeatmap
        {
            get
            {
                lock (_getlock)
                {
                    if (_getBeatmap == null)
                    {
                        Plugin.Log?.Info($"FAFA - GetBeatMap " + levelID);
                        _getBeatmap = BeatSaver.Client.Hash(levelID.Replace("custom_level_", ""));
                        _getBeatmap.ContinueWith(b =>
                        {
                            Populate(b.Result);
                        });
                    }
                }
                return _getBeatmap;
            }
        }
        public PreviewBeatmapLevelStub(string levelId)
        {
            levelID = levelId;
            Plugin.Log?.Info($"FAFA - Constructor 1 " + levelId);
        }

        public PreviewBeatmapLevelStub(string levelId, string songName, string levelAuthorName)
            : this(levelId)
        {
            this.songName = songName;
            this.levelAuthorName = levelAuthorName;
            Plugin.Log?.Info($"FAFA - Constructor 2 " + levelId);
        }

        private void Populate(Beatmap beatmap)
        {
            songName = beatmap.Metadata.SongName;
            songSubName = beatmap.Metadata.SongSubName;
            songAuthorName = beatmap.Metadata.SongAuthorName;
            levelAuthorName = beatmap.Metadata.LevelAuthorName;
            beatsPerMinute = beatmap.Metadata.BPM;
            songDuration = beatmap.Metadata.Duration;
        }

        public string levelID { get; private set; }

        public string songName  { get; private set; }

        public string songSubName  { get; private set; }

        public string songAuthorName  { get; private set; }

        public string levelAuthorName  { get; private set; }

        public float beatsPerMinute  { get; private set; }

        public float songTimeOffset  { get; private set; }

        public float shuffle  { get; private set; }

        public float shufflePeriod  { get; private set; }

        public float previewStartTime  { get; private set; }

        public float previewDuration  { get; private set; }

        public float songDuration  { get; private set; }

        public EnvironmentInfoSO environmentInfo  { get; private set; }

        public EnvironmentInfoSO allDirectionsEnvironmentInfo  { get; private set; }

        public PreviewDifficultyBeatmapSet[] previewDifficultyBeatmapSets  { get; private set; }

        public async Task<Sprite> GetCoverImageAsync(CancellationToken cancellationToken)
        {
            Beatmap bm = await GetBeatmap;
            var img = await bm.FetchCoverImage(cancellationToken);
            return Utilities.Utilities.GetSprite(img);
        }

        public Task<UnityEngine.AudioClip> GetPreviewAudioClipAsync(CancellationToken cancellationToken)
        {
            var bm = SongCore.Loader.GetLevelById(levelID);
            if(bm != null)
            {
                return bm.GetPreviewAudioClipAsync(cancellationToken);
            }
            return Task.FromResult<AudioClip>(null);
        }
    }
}
