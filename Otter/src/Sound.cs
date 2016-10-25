using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Otter {
    public class Sound {
        SoundEffect effect;

        public Sound(string path) {
            using (var stream = new FileStream(path, FileMode.Open))
                effect = SoundEffect.FromStream(stream);
        }

        public void Play() {
            effect.Play();
        }

        public void Play(float volume, float pitch = 1, float pan = 0) {
            effect.Play(volume, pitch, pan);
        }
    }
}
