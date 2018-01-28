#if false
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MusicVR.Wall
{
    public class MusicWall : MonoSingleton<MusicWall>, ICylinder
    {
        public event Action<MusicWallData> OnWallDataUpdated = (p) => { };

        public MusicWallData WallProperties;
        public bool HasWall = true;
        public float ProbInitSelected = 0.2f;
        public Synth Synth;

        private WallButtons m_wallButtons = new WallButtons();
        private WallMesh m_wallMesh;
        private WallMusicPlayer m_musicPlayer;
        private Transform m_transform;

        public bool NeedsUpdate { get; set; }
        public bool IsPlaying { get { return m_musicPlayer.IsPlaying; } }


        void Start()
        {
            m_transform = GetComponent<Transform>();
        }

        public void ClearWall()
        {
            WallProperties.CompositionData.Clear();
            NeedsUpdate = true;
        }


        public void GenerateRandom()
        {
            WallProperties.CompositionData.CreateDummyButtonData(ProbInitSelected);
            NeedsUpdate = true;
        }


        public void TogglePlayPause()
        {
            if (m_musicPlayer.IsPlaying)
                m_musicPlayer.Stop();
            else
                m_musicPlayer.Play();
        }

        #region ICylinderProperties

        public float GetCylinderRadius()
        {
            return WallProperties.Radius;
        }

        public Vector3 GetCylinderOrigin()
        {
            return m_transform.position;
        }

        #endregion

        protected override void _Awake()
        {
            if (HasWall)
                m_wallMesh = gameObject.AddComponent<WallMesh>();
            m_musicPlayer = gameObject.AddComponent<WallMusicPlayer>();
            WallProperties.Init();
            m_musicPlayer.Init(WallProperties, m_wallButtons, Synth);
            NeedsUpdate = true;
        }


        void OnValidate()
        {
            NeedsUpdate = true;
        }


        void Update()
        {
            if (NeedsUpdate)
            {
                m_wallButtons.Create(WallProperties);
                if (HasWall)
                    m_wallMesh.Create(WallProperties);
                m_musicPlayer.Init(WallProperties, m_wallButtons, Synth);
                m_musicPlayer.Reset();
                m_musicPlayer.Play();
                OnWallDataUpdated(WallProperties);

            }
            m_wallButtons.Update();
            NeedsUpdate = false;
        }
    }
}
#endif