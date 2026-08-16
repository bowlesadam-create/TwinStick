using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TwinStick
{
    internal class TiledMap
    {
        public class TiledObject
        {
            public string Name;
            public string Type;
            public Vector2 Position;
            public int Width;
            public int Height;
        }

        public List<TiledObject> Objects = new List<TiledObject>();

        public int Width;   // In Tiles
        public int Height;  // In Tiles
        public int TileWidth;
        public int TileHeight;

        public int[,] TileGrid;
        public Dictionary<int, bool> ColliadableLookup = new Dictionary<int, bool>();

        public static TiledMap Load(string filePath)
        {
            TiledMap map = new TiledMap();
            XDocument doc = XDocument.Load(filePath);
            XElement root = doc.Element("map");

            map.Width = (int)root.Attribute("width");
            map.Height = (int)root.Attribute("height");
            map.TileWidth = (int)root.Attribute("tilewidth");
            map.TileHeight = (int)root.Attribute("tileheight");



            // Tile Set Parsing (external .tsx file)
            XElement tilesetElement = root.Element("tileset");
            int firstGid = (int)tilesetElement.Attribute("firstgid");

            string tmxDirectory = System.IO.Path.GetDirectoryName(filePath);
            string tilesetSource = (string)tilesetElement.Attribute("source");
            string tilesetPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(tmxDirectory, tilesetSource));

            XDocument tilesetDoc = XDocument.Load(tilesetPath);
            XElement tilesetRoot = tilesetDoc.Element("tileset");

            foreach (XElement tileElement in tilesetRoot.Elements("tile"))
            {
                int localTileId = (int)tileElement.Attribute("id");
                int globalTileId = firstGid + localTileId;
                XElement propertiesElement = tileElement.Element("properties");
                if (propertiesElement != null)
                {
                    foreach (XElement propertyElement in propertiesElement.Elements("property"))
                    {
                        string propName = (string)propertyElement.Attribute("name");
                        if (propName == "Collidable")
                        {
                            bool isCollidable = (bool)propertyElement.Attribute("value");
                            map.ColliadableLookup[globalTileId] = isCollidable;
                        }
                    }
                }
            }

            // Tile Layer Parsing
            XElement layerElement = root.Element("layer");
            XElement dataElement = layerElement.Element("data");
            string csv = dataElement.Value.Trim();

            map.TileGrid = new int[map.Width,map.Height];
            string[] values = csv.Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            System.Diagnostics.Debug.WriteLine($"Width={map.Width}, Height={map.Height}, Expected cells={map.Width * map.Height}");
            System.Diagnostics.Debug.WriteLine($"values.Length={values.Length}");
            for (int i = 0; i < values.Length; i++) 
            {
                int tileId = int.Parse(values[i].Trim());
                int x = i % map.Width;
                int y = i / map.Width;
                map.TileGrid[x,y] = tileId;
            }

            XElement objectGroupElement = root.Element("objectgroup");
            System.Diagnostics.Debug.WriteLine($"Object group found: {objectGroupElement != null}");
            if (objectGroupElement != null) 
            {
                foreach (XElement objectElement in objectGroupElement.Elements("object"))
                {
                    TiledObject obj = new TiledObject();
                    obj.Name = (string)objectElement.Attribute("name") ?? "";

                    float x = (float)objectElement.Attribute("x");
                    float y = (float)objectElement.Attribute("y");
                    obj.Position = new Vector2(x, y);

                    obj.Width = objectElement.Attribute("width") != null ? (int)objectElement.Attribute("width") : 0;
                    obj.Height = objectElement.Attribute("height") != null ? (int)objectElement.Attribute("height") : 0;

                    XElement propertiesElement = objectElement.Element("properties");
                    if(propertiesElement != null)
                    {
                        foreach (XElement propertyElement in propertiesElement.Elements("property"))
                        {
                            string propName = (string)propertyElement.Attribute("name");
                            string propValue = (string)propertyElement.Attribute("value");

                            if (propName == "Type")
                            {
                                obj.Type = propValue;
                            }
                        }
                    }

                    map.Objects.Add(obj);
                }
            }
            System.Diagnostics.Debug.WriteLine($"Objects found: {map.Objects.Count}");
            foreach (var obj in map.Objects)
            {
                System.Diagnostics.Debug.WriteLine($"  Object: Name='{obj.Name}', Type='{obj.Type}', Pos={obj.Position}");
            }
            return map;
        }

        public bool IsCollidableAt(int tileX, int tileY)
        {
            if (tileX < 0 || tileY < 0 || tileX >= Width || tileY >= Height)
                return true; // stop from going out of bounds

            int tileId = TileGrid[tileX,tileY];
            return ColliadableLookup.TryGetValue(tileId, out bool collidable) && collidable;
        }

        public void Draw(SpriteBatch sb, Texture2D floorTexture, Texture2D wallTexture)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int tileId = TileGrid[x, y];
                    if (tileId == 0) continue; // empty cell

                    bool isWall = ColliadableLookup.TryGetValue(tileId, out bool collidable) && collidable;
                    Texture2D texture = isWall ? wallTexture : floorTexture;
                    
                    Vector2 position = new Vector2(x*TileWidth, y * TileHeight);
                    sb.Draw(texture, position, Color.White);
                }
            }
        }
    }
}
