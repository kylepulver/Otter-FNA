using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Draw {
        public Surface TargetSurface { get; private set; }
        internal Surface DefaultTargetSurface;
        internal float layerDepth = 1;
        internal SamplerState SamplerState = SamplerState.PointClamp;
        internal Core Core;
        internal Matrix TransformMatrix;

        public void SetTarget(Surface surface) {
            if (surface == null) {
                ResetTarget();
                return;
            }

            TargetSurface = surface;
            Core.GraphicsDevice.SetRenderTarget(TargetSurface.Target);
        }

        public void ResetTarget() {
            TargetSurface = DefaultTargetSurface;
            Core.GraphicsDevice.SetRenderTarget(TargetSurface.Target);
        }

        public void Clear(Color color) {
            Core.GraphicsDevice.Clear(color.ToXnaColor());
        }

        public void Texture(Texture texture, float x, float y) {
            Texture(texture, x, y, Color.White);
        }

        public void Texture(Texture texture, float x, float y, Color color) {
            Texture(
                texture,
                new Vector2(x, y),
                color
                );
        }

        public void Texture(Texture texture, Vector2 position, Color color) {
            Texture(
                texture,
                texture.Bounds,
                position,
                color
                );
        }

        public void Texture(Texture texture, Rectangle sourceRect, Vector2 position, Color color) {
            Texture(
                texture,
                sourceRect,
                position,
                Vector2.One,
                0,
                Vector2.Zero,
                color
                );
        }

        public void Texture(Texture texture, int sourceX, int sourceY, int sourceWidth, int sourceHeight, Vector2 position, Color color) {
            Texture(
                texture, 
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), 
                position, 
                color
                );
        }

        public void Texture(Texture texture, int sourceX, int sourceY, int sourceWidth, int sourceHeight, float x, float y, Color color) {
            Texture(
                texture,
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), 
                new Vector2(x, y), 
                color
                );
        }

        // NOTE: THIS ONE ACTUALLY DRAWS >:I
        public void Texture(Texture texture, Rectangle sourceRect, Vector2 position, Vector2 scale, float rotation, Vector2 origin, Color color, Shader shader = null, bool flipX = false, bool flipY = false) {
            rotation = rotation * -Util.DEG_TO_RAD;

            var effects = SpriteEffects.None;
            if (flipX) effects |= SpriteEffects.FlipHorizontally;
            if (flipY) effects |= SpriteEffects.FlipVertically;

            if (shader != null)
                Begin(shader);
            else
                Begin();

            SpriteBatch.Draw(
                texture.XnaTexture,
                position,
                sourceRect.ToXnaRectangle(),
                color.ToXnaColor(),
                rotation,
                origin,
                scale,
                effects,
                layerDepth
                );

            End();
            layerDepth -= 0.0000001f; // make this not totally stupid :)
        }

        public void Texture(Texture texture, int sourceX, int sourceY, int sourceWidth, int sourceHeight, Vector2 position, Vector2 scale, float rotation, Vector2 origin, Color color, Shader shader) {
            Texture(texture, new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), position, scale, rotation, origin, color, shader);
        }

        SpriteBatch SpriteBatch {
            get { return Core.SpriteBatch; }
        }

        internal void ResetMatrix() {
            TransformMatrix = GetTransformMatrix(0, 0, 1, 1, 0, 0, 0);
        }

        internal void Begin(Shader shader) {
            var matrix = TransformMatrix * TargetSurface.GetCameraTransform();
            SpriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.NonPremultiplied,
                SamplerState, // Todo: control smoothing options
                null,
                null,
                shader.ToXnaEffect(),
                matrix.ToXnaMatrix()
                );
        }

        //internal void Begin(Vector2 translate, Vector2 scale, float rotation, Vector2 origin) {
        //    var matrix = GetTransformMatrix(translate, scale, rotation, origin);
        //    matrix *= TargetSurface.GetCameraTransform();
        //    SpriteBatch.Begin(
        //        SpriteSortMode.BackToFront,
        //        BlendState.NonPremultiplied,
        //        SamplerState,
        //        null,
        //        null,
        //        null,
        //        matrix.ToXnaMatrix()
        //        );
        //}

        internal void Begin() {
            var matrix = TransformMatrix * TargetSurface.GetCameraTransform();
            SpriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.NonPremultiplied,
                SamplerState,
                null,
                null,
                null,
                matrix.ToXnaMatrix()
                );
        }

        internal void BeginNoCamera() {
            SpriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.NonPremultiplied,
                SamplerState,
                null,
                null
                );
        }

        internal void End() {
            SpriteBatch.End();
        }

        internal struct DrawState : IEquatable<DrawState> {
            public SpriteSortMode SpriteSortMode;
            public BlendMode BlendMode;
            public Shader Shader;
            public SamplerState SamplerState;

            public DrawState(BlendMode blendMode, Shader shader, SamplerState samplerState) {
                Shader = shader;
                BlendMode = blendMode;
                SpriteSortMode = SpriteSortMode.BackToFront;
                SamplerState = samplerState;
            }

            public bool Equals(DrawState other) {
                if (Shader != other.Shader) return false;
                if (BlendMode != other.BlendMode) return false;
                if (SamplerState.Filter != other.SamplerState.Filter) return false;
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
