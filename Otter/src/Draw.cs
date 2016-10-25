using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Draw {
        public Surface TargetSurface { get; private set; }
        Surface defaultTarget;

        public float CameraX;
        public float CameraY;

        //public Otter.Vector2 Camera {
        //    get { return new Otter.Vector2(CameraX, CameraY); }
        //    set { CameraX = value.X; CameraY = value.Y; }
        //}

        public float CameraRotation;
        public float CameraZoom = 1;

        public void SetTarget(Surface surface) {
            TargetSurface = surface;
        }

        public void ResetTarget() {
            TargetSurface = defaultTarget;
        }

        public void Clear(Color color) {
            Core.Instance.GraphicsDevice.Clear(color.ToXnaColor());
        }

        public void Texture(Texture texture, float x, float y, Color color) {
            Texture(texture, new Vector2(x, y), color);
        }

        public void Texture(Texture texture, Vector2 position, Color color) {
            SpriteBatch.Draw(texture.XnaTexture, position, color.ToXnaColor());
        }

        public void Texture(Texture texture, Rectangle sourceRect, Vector2 position, Color color) {
            SpriteBatch.Draw(texture.XnaTexture, position, sourceRect.ToXnaRectangle(), color.ToXnaColor());
        }

        public void Texture(Texture texture, int sourceX, int sourceY, int sourceWidth, int sourceHeight, Vector2 position, Color color) {
            Texture(texture, new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), position, color);
        }

        public void Texture(Texture texture, int sourceX, int sourceY, int sourceWidth, int sourceHeight, float x, float y, Color color) {
            Texture(texture, new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), new Vector2(x, y), color);
        }

        public void Texture(Texture texture, Rectangle sourceRect, Vector2 position, Color color, Shader shader) {
            End();
            // todo: make this not suck
            Begin(shader);
            Texture(texture, sourceRect, position, color);
            End();
            Begin();
        }

        SpriteBatch SpriteBatch {
            get { return Core.Instance.SpriteBatch; }
        }

        internal void Begin(Shader shader) {
            SpriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.NonPremultiplied,
                null,
                null,
                null,
                shader.ToXnaEffect(),
                GetCameraTransform().ToXnaMatrix());
        }

        internal void Begin() {
            SpriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.NonPremultiplied,
                null,
                null,
                null,
                null,
                GetCameraTransform().ToXnaMatrix());
        }

        internal void End() {
            SpriteBatch.End();
        }

        public struct DrawState : IEquatable<DrawState> {
            public SpriteSortMode SpriteSortMode;
            public BlendMode BlendMode;
            public Shader Shader;

            public DrawState(BlendMode blendMode, Shader shader) {
                Shader = shader;
                BlendMode = blendMode;
                SpriteSortMode = SpriteSortMode.BackToFront;
            }

            public bool Equals(DrawState other) {
                if (Shader != other.Shader) return false;
                if (BlendMode != other.BlendMode) return false;
                return true;
            }
        }

        internal Matrix GetCameraTransform() {
            return Matrix.CreateTranslation(-CameraX - Game.Instance.HalfWidth, -CameraY - Game.Instance.HalfHeight, 0) *
                Matrix.CreateRotationZ(-CameraRotation * Util.DEG_TO_RAD) *
                Matrix.CreateScale(CameraZoom, CameraZoom, 1) *
                Matrix.CreateTranslation(Game.Instance.HalfWidth, Game.Instance.HalfHeight, 0);
        }
    }

    public enum BlendMode {
        Alpha,
        Add,
        None
    }

}
