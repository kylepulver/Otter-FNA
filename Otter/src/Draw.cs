﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Draw {
        public Surface TargetSurface { get; private set; }
        internal Surface DefaultTargetSurface;

        public void SetTarget(Surface surface) {
            TargetSurface = surface;
        }

        public void ResetTarget() {
            TargetSurface = DefaultTargetSurface;
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

        public void Texture(Texture texture, Rectangle sourceRect, Vector2 position, Vector2 scale, float rotation, Vector2 origin, Color color, Shader shader) {
            if (shader != null) {
                End();
                Begin(shader);
                SpriteBatch.Draw(texture.XnaTexture, position, sourceRect.ToXnaRectangle(), color.ToXnaColor(), rotation, origin, scale, SpriteEffects.None, 0);
                End();
                Begin();
            }
            else {
                SpriteBatch.Draw(texture.XnaTexture, position, sourceRect.ToXnaRectangle(), color.ToXnaColor(), rotation, origin, scale, SpriteEffects.None, 0);
            }
        }

        public void Texture(Texture texture, int sourceX, int sourceY, int sourceWidth, int sourceHeight, Vector2 position, Vector2 scale, float rotation, Vector2 origin, Color color, Shader shader) {
            Texture(texture, new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), position, scale, rotation, origin, color, shader);
        }


        public void Texture(Texture texture, Vector2 position, Vector2 scale, float rotation, Vector2 origin, Color color, Shader shader) {
            
        }

        public void Texture(Texture texture, Rectangle sourceRect, float x, float y, float scaleX, float scaleY, float rotation, float originX, float originY, Color color, Shader shader) {
            
        }

        public void Texture(Texture texture, int sourceX, int sourceY, int sourceWidth, int sourceHeight, float x, float y, float scaleX, float scaleY, float rotation, float originX, float originY, Color color, Shader shader) {
            
        }

        public void Texture(Texture texture, float x, float y, float scaleX, float scaleY, float rotation, float originX, float originY, Color color, Shader shader) {
            
        }

        public void Texture(Texture texture, Rectangle sourceRect, Vector2 position, Color color, Shader shader) {
            
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
                TargetSurface.GetCameraTransform().ToXnaMatrix());
        }

        internal void Begin() {
            SpriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.NonPremultiplied,
                null,
                null,
                null,
                null,
                TargetSurface.GetCameraTransform().ToXnaMatrix());
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

        internal Matrix GetTransformMatrix(Vector2 translation, Vector2 scale, float rotation, Vector2 origin) {
            return
                Matrix.CreateTranslation(-origin.X, -origin.Y, 0) *
                Matrix.CreateRotationZ(-rotation * Util.DEG_TO_RAD) *
                Matrix.CreateScale(scale.X, scale.Y, 1) *
                Matrix.CreateTranslation(translation.X + origin.X, translation.Y + origin.Y, 0);
        }

        internal Matrix GetTransformMatrix(float x, float y, float scaleX, float scaleY, float rotation, float originX, float originY) {
            return GetTransformMatrix(new Vector2(x, y), new Vector2(scaleX, scaleY), rotation, new Vector2(originX, originY));
        }
    }

    public enum BlendMode {
        Alpha,
        Add,
        None
    }

}
