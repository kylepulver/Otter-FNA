using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    class Resources {
        static Dictionary<string, Texture2D> loadedTextures = new Dictionary<string, Texture2D>();

        internal static Texture2D GetTexture2D(string path) {
            if (!loadedTextures.ContainsKey(path))
                using (var stream = new FileStream(path, FileMode.Open))
                    loadedTextures.Add(path, Texture2D.FromStream(Core.Instance.GraphicsDevice, stream));
           
            return loadedTextures[path];
        }

        internal static void OnGraphicsReady(Action<GraphicsDevice> action) {
            if (Core.Instance.IsReady) action(Core.Instance.GraphicsDevice);
            else Core.Instance.OnGraphicsDeviceReady += action;
        }

    }
}
