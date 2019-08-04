using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using Newtonsoft.Json;
using TRY.Kampfmodus.Collision;

namespace TRY.Kampfmodus
{
    internal sealed class Map
    {
        private readonly bool mTechDemo;
        private readonly Random mRandom;
        private readonly TiledMap mBackground;
        private readonly TiledMap mLimits;
        private TiledMap mOl;
        private TiledMap mUl;
        private TiledMap mOr;
        private TiledMap mUr;
        private TiledMap mStartZone;
        private TiledMap mEndZone;
        private readonly TiledMap mEndboss;
        private readonly List<TiledMap> mStartZones;
        private readonly List<TiledMap> mEndZones;
        private readonly List<TiledMap> mOls;
        private readonly List<TiledMap> mUls;
        private readonly List<TiledMap> mOrs;
        private readonly List<TiledMap> mUrs;
        private readonly List<TiledMap> mMapList;
        private readonly List<TiledMap> mLevelList;
        private readonly TiledMapRenderer mMapRenderer;
        private readonly TiledMap mTech;

        public TiledMapObject[] TiledMapObjects { get; private set; }
        public int HeightInPixels => mBackground.HeightInPixels;
        public int WidthInPixels => mBackground.WidthInPixels;
        public int TileWidth => mBackground.TileWidth;
        public int Width => mBackground.Width;
        public int Height => mBackground.Height;
        public List<Vector2> ItemPosition { get; }
        public HashSet<Vector2> EnemySpawns { get; }
        private List<int> MapOrient { get; set; }

        public Map(ContentManager content, GraphicsDevice graphicsDevice)
        {
            mTechDemo = false;
            mRandom = new Random();
            mMapList = new List<TiledMap>();
            mLevelList = new List<TiledMap>();
            ItemPosition = new List<Vector2>();
            EnemySpawns = new HashSet<Vector2>();
            mLimits = content.Load<TiledMap>("Map/Limits/OU");
            mBackground = content.Load<TiledMap>("Map/Background");
            mEndboss = content.Load<TiledMap>("Map/Endboss/Endboss");
            mTech = content.Load<TiledMap>("Map/Techdemo");
            mMapList.Add(mLimits);

            mStartZones = new List<TiledMap>
            {
                content.Load<TiledMap>("Map/Startzone/L"),
                content.Load<TiledMap>("Map/Startzone/L1"),
                content.Load<TiledMap>("Map/Startzone/L2"),
            };

            mEndZones = new List<TiledMap>
            {
                content.Load<TiledMap>("Map/Endzone/R")
            };

            mOls = new List<TiledMap>
            {
                content.Load<TiledMap>("Map/Ol/0"),
                content.Load<TiledMap>("Map/Ol/1"),
                content.Load<TiledMap>("Map/Ol/2"),
                content.Load<TiledMap>("Map/Ol/3"),
            };

            mUls = new List<TiledMap>
            {
                content.Load<TiledMap>("Map/Ul/0"),
                content.Load<TiledMap>("Map/Ul/1"),
                content.Load<TiledMap>("Map/Ul/2"),
                content.Load<TiledMap>("Map/Ul/3")
            };

            mOrs = new List<TiledMap>
            {
                content.Load<TiledMap>("Map/Or/0"),
                content.Load<TiledMap>("Map/Or/1"),
                content.Load<TiledMap>("Map/Or/2"),
                content.Load<TiledMap>("Map/Or/3")
            };

            mUrs = new List<TiledMap>
            {
                content.Load<TiledMap>("Map/Ur/0"),
                content.Load<TiledMap>("Map/Ur/1"),
                content.Load<TiledMap>("Map/Ur/2"),
                content.Load<TiledMap>("Map/Ur/3")
            };

            mMapRenderer = new TiledMapRenderer(graphicsDevice);
        }

        public TiledMapObjectLayer EnemyStart()
        {
            return mOl.GetLayer<TiledMapObjectLayer>("EnemyStart");
        }

        private void ChooseStartZone()
        {
            int i = mRandom.Next(mStartZones.Count);
            mStartZone = mStartZones[i];
            mMapList.Add(mStartZone);
            MapOrient.Add(i);
        }

        private void ChooseEndZone()
        {
            int i = mRandom.Next(mEndZones.Count);
            mEndZone = mEndZones[i];
            mMapList.Add(mEndZone);
            MapOrient.Add(i);
        }

        private void ChooseOl()
        {
            var i  = mRandom.Next(mOls.Count);
            mOl = mOls[i];
            mMapList.Add(mOl); 
            mLevelList.Add(mOl); 
            MapOrient.Add(i);
        }

        private void ChooseUl()
        {
            var i = mRandom.Next(mUls.Count);
            mUl = mUls[i];
            mMapList.Add(mUl);
            mLevelList.Add(mUl);
            MapOrient.Add(i);
        }

        private void ChooseOr()
        {
            var i = mRandom.Next(mOrs.Count);
            mOr = mOrs[i];
            mMapList.Add(mOr);
            mLevelList.Add(mOr);
            MapOrient.Add(i);
        }

        private void ChooseUr()
        {
            var i = mRandom.Next(mUrs.Count);
            mUr = mUrs[i];
            mMapList.Add(mUr);
            mLevelList.Add(mUr);
            MapOrient.Add(i);
        }

        public void GenerateMap(int level, bool load = false)
        {
            string s = Directory.GetCurrentDirectory();
            if (level == 26)
            {
                mMapList.Add(mTech);
                GetItems();
                GetCollisionObjects();
            }
            else if (level == 24)
            {
                mMapList.Add(mEndboss);
                GetItems();
                GetCollisionObjects();
                GetEnemySpawns();
            }
            else
            {
                
                if (load)
                {
                    MapOrient = JsonConvert.DeserializeObject<List<int>>(File.ReadAllText(s + "\\map.txt"));

                    mStartZone = mStartZones[MapOrient[0]];
                    mEndZone = mEndZones[MapOrient[1]];
                    mOl = mOls[MapOrient[2]];
                    mUl = mUls[MapOrient[3]];
                    mOr = mOrs[MapOrient[4]];
                    mUr = mUrs[MapOrient[5]];

                    mMapList.Add(mOl);
                    mLevelList.Add(mOl);
                    mMapList.Add(mUl);
                    mLevelList.Add(mUl);
                    mMapList.Add(mOr);
                    mLevelList.Add(mOr);
                    mMapList.Add(mUr);
                    mLevelList.Add(mUr);
                    mMapList.Add(mStartZone);
                    mMapList.Add(mEndZone);

                }
                else
                {
                    MapOrient = new List<int>();
                    ChooseStartZone();
                    ChooseEndZone();
                    ChooseOl();
                    ChooseUl();
                    ChooseOr();
                    ChooseUr();
                }
                GetItems();
                GetCollisionObjects();
                GetEnemySpawns();
                EnemyStart();
                string output = JsonConvert.SerializeObject(MapOrient);
                StreamWriter sw = new StreamWriter(@s+"\\map.txt");
                sw.WriteLine(output);
                sw.Close();
            }
        }

        public void GetDoors(TextureManager textureManager, BattleModeState bms,
                             CollisionManager collisionManager)
        {
            foreach (var map in mMapList)
            {
                var doorLayerLeft = map.GetLayer<TiledMapObjectLayer>("LeftDoors");
                var doorLayerRight = map.GetLayer<TiledMapObjectLayer>("RightDoors");
                doorLayerRight.IsVisible = false;
                doorLayerLeft.IsVisible = false;
                for (var index = 0; index < Math.Max(doorLayerLeft.Objects.Length,
                     doorLayerRight.Objects.Length); index++)
                {
                    Door leftDoor = new Door(textureManager.GetTexture("DoorHorizontal"),
                            doorLayerLeft.Objects[index].Position,
                            true);

                    bms.AddDoor(leftDoor);
                    collisionManager.AddCollider(leftDoor);
                }
            }
        }

        private void GetItems()
        {
            foreach (var map in mLevelList)
            {
                var items = map.GetLayer<TiledMapObjectLayer>("Items");
                if (items?.Objects == null) continue;
                foreach (var item in items.Objects)
                {
                    ItemPosition.Add(item.Position);
                }
            }
        }

        private void GetEnemySpawns()
        {
            foreach (var map in mLevelList)
            {
                var items = map.GetLayer<TiledMapObjectLayer>("EnemySpawn");
                if (items?.Objects == null) continue;
                foreach (var item in items.Objects)
                {
                    EnemySpawns.Add(item.Position);
                }
            }
        }

        private void GetCollisionObjects()
        {
            var collider = new List<TiledMapObject>();
            foreach (var map in mMapList)
            {
                var collisionLayer = map.GetLayer<TiledMapObjectLayer>("CollisionObjects");
                collider.AddRange(collisionLayer.Objects);
            }
            TiledMapObjects = collider.ToArray();
        }

        public TiledMapObjectLayer Objects(int level)
        {
            if (level == 24) return mEndboss.GetLayer<TiledMapObjectLayer>("Objects");
            else if (level == 26) return mTech.GetLayer<TiledMapObjectLayer>("Objects");
            else return mStartZone.GetLayer<TiledMapObjectLayer>("Objects");
        }

        public TiledMapObject GetWinZone(int level)
        {
            return level == 24 ? mEndboss.GetLayer<TiledMapObjectLayer>("wincondition").Objects[0] :
                                 mEndZone.GetLayer<TiledMapObjectLayer>("wincondition").Objects[0];
        }

        public void Update(GameTime gameTime, int level)
        {
            if (level == 26) mMapRenderer.Update(mTech, gameTime);
            else if (level == 24)
            {
                mMapRenderer.Update(mEndboss, gameTime);
            }
            else
            {
                mMapRenderer.Update(mBackground, gameTime);
                mMapRenderer.Update(mLimits, gameTime);
                mMapRenderer.Update(mStartZone, gameTime);
                mMapRenderer.Update(mEndZone, gameTime);
                mMapRenderer.Update(mOl, gameTime);
                mMapRenderer.Update(mOr, gameTime);
                mMapRenderer.Update(mUl, gameTime);
                mMapRenderer.Update(mUr, gameTime);
            }
        }

        public void Draw(Camera camera, int level)
        {
            if (level == 26) mMapRenderer.Draw(mTech, camera.Transform);
            else if (level == 24)
            {
                mMapRenderer.Draw(mEndboss, camera.Transform);
            }
            else
            {
                mMapRenderer.Draw(mBackground, camera.Transform);
                mMapRenderer.Draw(mLimits, camera.Transform);
                mMapRenderer.Draw(mStartZone, camera.Transform);
                mMapRenderer.Draw(mEndZone, camera.Transform);
                if (mTechDemo) return;
                mMapRenderer.Draw(mOl, camera.Transform);
                mMapRenderer.Draw(mUl, camera.Transform);
                mMapRenderer.Draw(mOr, camera.Transform);
                mMapRenderer.Draw(mUr, camera.Transform);

            }
        }
    }
}
