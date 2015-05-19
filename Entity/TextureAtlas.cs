using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Microsoft.Xna.Framework;
using CardioCritters;

namespace XnaUtility
{
	public class TextureAtlas
	{
		public Dictionary<string, TextureRegion> Regions { get; private set; }

		public TextureAtlas(string fileName)
		{
            fileName = Utility.Content.RootDirectory +
#if WINDOWS
                "\\"
#elif ANDROID
                "/"
#endif
                + fileName;
			InitializeFromXml(fileName);
		}

		private void InitializeFromXml(string fileName)
		{
			Regions = new Dictionary<string, TextureRegion>();
            using (var stream = TitleContainer.OpenStream(fileName))
            {
			    XDocument xDocument = XDocument.Load(stream);
			    foreach (var sprite in xDocument.Descendants("sprite"))
			    {
				    TextureRegion textureRegion = new TextureRegion();

				    int x = int.Parse(sprite.Attribute("x").Value, CultureInfo.InvariantCulture);
				    int y = int.Parse(sprite.Attribute("y").Value, CultureInfo.InvariantCulture);
				    int w = int.Parse(sprite.Attribute("w").Value, CultureInfo.InvariantCulture);
				    int h = int.Parse(sprite.Attribute("h").Value, CultureInfo.InvariantCulture);
				    int oX = (sprite.Attribute("oX") == null) ? 0 : int.Parse(sprite.Attribute("oX").Value, CultureInfo.InvariantCulture);
				    int oY = (sprite.Attribute("oY") == null) ? 0 : int.Parse(sprite.Attribute("oY").Value, CultureInfo.InvariantCulture);
				    int oW = (sprite.Attribute("oW") == null) ? w : int.Parse(sprite.Attribute("oW").Value, CultureInfo.InvariantCulture);
				    int oH = (sprite.Attribute("oH") == null) ? h : int.Parse(sprite.Attribute("oH").Value, CultureInfo.InvariantCulture);

				    textureRegion.Bounds = new Microsoft.Xna.Framework.Rectangle(x, y, w, h);
				    textureRegion.Rotated = sprite.Attribute("r") != null;

				    textureRegion.OriginTopLeft = new Vector2(-oX, -oY);
				    textureRegion.OriginCenter = new Vector2(((oW / 2.0f) - (oX)), ((oH / 2.0f) - (oY)));
				    textureRegion.OriginBottomRight = new Vector2((oW - (oX)), (oH - (oY)));

				    Regions[sprite.Attribute("n").Value] = textureRegion;
			    }
            }
		}

		internal class Rectangle
		{
			public int x { get; set; }
			public int y { get; set; }
			public int w { get; set; }
			public int h { get; set; }
		}

		internal class Dimensions
		{
			public int w { get; set; }
			public int h { get; set; }
		}

		internal class Frame
		{
			public string filename { get; set; }
			public Rectangle frame { get; set; }
			public bool rotated { get; set; }
			public bool trimmed { get; set; }
			public Rectangle spriteSourceSize { get; set; }
			public Dimensions sourceSize { get; set; }
		}

		internal class Meta
		{
			public string app { get; set; }
			public string version { get; set; }
			public string image { get; set; }
			public string format { get; set; }
			public Dimensions size { get; set; }
			public string scale { get; set; }
			public string smartupdate { get; set; }
		}

		internal class TextureDictionaryData
		{
			public Frame[] frames { get; set; }
			public Meta meta { get; set; }
		}

        public bool ContainsTexture(string textureName)
        {
            return Regions.ContainsKey(textureName);
        }

        public TextureRegion GetRegion(string textureName)
        {
            TextureRegion region;

            if (Regions.TryGetValue(textureName, out region))
                return region;
            return null;
        }

	}
}

