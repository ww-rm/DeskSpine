﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpineRuntime38;

namespace Spine
{
    internal class Spine38 : Spine
    {
        private class TextureLoader : SpineRuntime38.TextureLoader
        {
            public void Load(AtlasPage page, string path)
            {
                var texture = new SFML.Graphics.Texture(path);
                if (page.magFilter == TextureFilter.Linear)
                    texture.Smooth = true;
                if (page.uWrap == TextureWrap.Repeat && page.vWrap == TextureWrap.Repeat)
                    texture.Repeated = true;
                
                page.rendererObject = texture;
                page.width = (int)texture.Size.X;
                page.height = (int)texture.Size.Y;
            }

            public void Unload(object texture)
            {
                ((SFML.Graphics.Texture)texture).Dispose();
            }
        }

        private static TextureLoader textureLoader = new();

        private Atlas atlas;
        private SkeletonBinary? skeletonBinary;
        private SkeletonJson? skeletonJson;
        private SkeletonData skeletonData;
        private AnimationStateData animationStateData;

        private Skeleton skeleton;
        private AnimationState animationState;

        private SkeletonClipping clipping = new();

        public override SpineVersion Version { get => SpineVersion.V38; }

        public Spine38(string skelPath, string? atlasPath = null, float defaultMix = 0) : base(skelPath, atlasPath)
        {
            atlas = new Atlas(AtlasPath, textureLoader);
            if (Path.GetExtension(SkelPath) == ".skel")
            {
                skeletonJson = null;
                skeletonBinary = new SkeletonBinary(atlas);
                skeletonData = skeletonBinary.ReadSkeletonData(SkelPath);
            }
            else if (Path.GetExtension(SkelPath) == ".json")
            {
                skeletonBinary = null;
                skeletonJson = new SkeletonJson(atlas);
                skeletonData = skeletonJson.ReadSkeletonData(SkelPath);
            }
            else
            {
                throw new ArgumentException($"Unknown skeleton file format {SkelPath}");
            }
            animationStateData = new AnimationStateData(skeletonData) { DefaultMix = defaultMix };
            skeleton = new Skeleton(skeletonData);
            animationState = new AnimationState(animationStateData);

            foreach (var anime in skeletonData.Animations)
                animationNames.Add(anime.Name);

            CurrentAnimation = DefaultAnimationName;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            atlas.Dispose();
        }

        public override float Scale
        {
            get
            {
                if (skeletonBinary is not null)
                    return skeletonBinary.Scale;
                else if (skeletonJson is not null)
                    return skeletonJson.Scale;
                else
                    return 1f;
            }
            set
            {
                // 保存状态
                var flipX = FlipX;
                var flipY = FlipY;
                var x = X;
                var y = Y;
                var savedTrack0 = animationState.GetCurrent(0);

                var val = Math.Clamp(value, ScaleMin, ScaleMax);
                if (skeletonBinary is not null)
                {
                    skeletonBinary.Scale = val;
                    skeletonData = skeletonBinary.ReadSkeletonData(SkelPath);
                }
                else if (skeletonJson is not null)
                {
                    skeletonJson.Scale = val;
                    skeletonData = skeletonJson.ReadSkeletonData(SkelPath);
                }

                // reload skel-dependent data
                animationStateData = new AnimationStateData(skeletonData) { DefaultMix = animationStateData.DefaultMix };
                skeleton = new Skeleton(skeletonData);
                animationState = new AnimationState(animationStateData);

                // 恢复状态
                FlipX = flipX;
                FlipY = flipY;
                X = x;
                Y = y;

                // 恢复原本 Track0 上所有动画
                if (savedTrack0 is not null)
                {
                    var entry = animationState.SetAnimation(0, savedTrack0.Animation.Name, true);
                    entry.TrackTime = savedTrack0.TrackTime;
                    var savedEntry = savedTrack0.Next;
                    while (savedEntry is not null)
                    {
                        entry = animationState.AddAnimation(0, savedEntry.Animation.Name, true, 0);
                        entry.TrackTime = savedEntry.TrackTime;
                        savedEntry = savedEntry.Next;
                    }
                }
            }
        }

        public override bool FlipX
        {
            get => skeleton.ScaleX < 0;
            set
            {
                if (skeleton.ScaleX > 0 && value || skeleton.ScaleX < 0 && !value)
                    skeleton.ScaleX *= -1;
            }
        }

        public override bool FlipY
        {
            get => skeleton.ScaleY < 0;
            set
            {
                if (skeleton.ScaleY > 0 && value || skeleton.ScaleY < 0 && !value)
                    skeleton.ScaleY *= -1;
            }
        }

        public override float X { get => skeleton.X; set => skeleton.X = value; }

        public override float Y { get => skeleton.Y; set => skeleton.Y = value; }

        public override float GetAnimationDuration(string name) { return skeletonData.FindAnimation(name)?.Duration ?? 0f; }

        public override string CurrentAnimation
        {
            get => animationState.GetCurrent(0)?.Animation.Name ?? DefaultAnimationName;
            set { if (animationNames.Contains(value)) animationState.SetAnimation(0, value, true); }
        }

        public override void AddAnimation(string name)
        {
            if (animationNames.Contains(name))
                animationState.AddAnimation(0, name, true, 0);
        }

        public override void Update(float delta)
        {
            skeleton.Update(delta);
            animationState.Update(delta);
            animationState.Apply(skeleton);
            skeleton.UpdateWorldTransform();
        }

        private SFML.Graphics.BlendMode GetSFMLBlendMode(BlendMode spineBlendMode)
        {
            return spineBlendMode switch
            {
                BlendMode.Normal => SFMLBlendMode.Normal,
                BlendMode.Additive => SFMLBlendMode.Additive,
                BlendMode.Multiply => SFMLBlendMode.Multiply,
                BlendMode.Screen => SFMLBlendMode.Screen,
                _ => throw new NotImplementedException($"{spineBlendMode}"),
            };
        }

        public override void Draw(SFML.Graphics.RenderTarget target, SFML.Graphics.RenderStates states)
        {
            vertexArray.Clear();
            states.Texture = null;

            // 要用 DrawOrder 而不是 Slots
            foreach (var slot in skeleton.DrawOrder)
            {
                var attachment = slot.Attachment;
                if (attachment is null)
                { 
                    clipping.ClipEnd(slot); 
                    continue; 
                }

                SFML.Graphics.Texture texture;
                
                float[] worldVertices = worldVerticesBuffer;    // 顶点世界坐标, 连续的 [x0, y0, x1, y1, ...] 坐标值
                int worldVerticesCount;                         // 等于顶点数组的长度除以 2
                int[] worldTriangleIndices;                     // 三角形索引, 从顶点坐标数组取的时候要乘以 2, 最大值是 worldVerticesCount - 1
                int worldTriangleIndicesLength;                 // 三角形索引数组长度
                float[] uvs;                                    // 纹理坐标
                float tintR = skeleton.R * slot.R;
                float tintG = skeleton.G * slot.G;
                float tintB = skeleton.B * slot.B;
                float tintA = skeleton.A * slot.A;

                if (attachment is RegionAttachment regionAttachment)
                {
                    texture = (SFML.Graphics.Texture)((AtlasRegion)regionAttachment.RendererObject).page.rendererObject;

                    regionAttachment.ComputeWorldVertices(slot.Bone, worldVertices, 0);
                    worldVerticesCount = 4;
                    worldTriangleIndices = [0, 1, 2, 2, 3, 0];
                    worldTriangleIndicesLength = 6;
                    uvs = regionAttachment.UVs;
                    tintR *= regionAttachment.R;
                    tintG *= regionAttachment.G;
                    tintB *= regionAttachment.B;
                    tintA *= regionAttachment.A;
                }
                else if (attachment is MeshAttachment meshAttachment)
                {
                    texture = (SFML.Graphics.Texture)((AtlasRegion)meshAttachment.RendererObject).page.rendererObject;

                    if (meshAttachment.WorldVerticesLength > worldVertices.Length)
                        worldVertices = worldVerticesBuffer = new float[meshAttachment.WorldVerticesLength * 2];
                    meshAttachment.ComputeWorldVertices(slot, worldVertices);
                    worldVerticesCount = meshAttachment.WorldVerticesLength / 2;
                    worldTriangleIndices = meshAttachment.Triangles;
                    worldTriangleIndicesLength = meshAttachment.Triangles.Length;
                    uvs = meshAttachment.UVs;
                    tintR *= meshAttachment.R;
                    tintG *= meshAttachment.G;
                    tintB *= meshAttachment.B;
                    tintA *= meshAttachment.A;
                }
                else if (attachment is ClippingAttachment clippingAttachment)
                {
                    clipping.ClipStart(slot, clippingAttachment);
                    continue;
                }
                else
                {
                    continue;
                }

                SFML.Graphics.BlendMode blendMode = GetSFMLBlendMode(slot.Data.BlendMode);

                states.Texture ??= texture;
                if (states.BlendMode != blendMode || states.Texture != texture)
                {
                    if (vertexArray.VertexCount > 0)
                    {
                        // XXX: 实测不用设置 sampler2D 的值也正确
                        if (UsePremultipliedAlpha && (states.BlendMode == SFMLBlendMode.Normal || states.BlendMode == SFMLBlendMode.Additive))
                            states.Shader = premultipliedAlphaFragmentShader;
                        else
                            states.Shader = null;
                        target.Draw(vertexArray, states);
                        vertexArray.Clear();
                    }
                    states.BlendMode = blendMode;
                    states.Texture = texture;
                }

                if (clipping.IsClipping)
                {
                    // 这里必须单独记录 Count, 和 Items 的 Length 是不一致的
                    clipping.ClipTriangles(worldVertices, worldVerticesCount * 2, worldTriangleIndices, worldTriangleIndicesLength, uvs);
                    worldVertices = clipping.ClippedVertices.Items;
                    worldVerticesCount = clipping.ClippedVertices.Count / 2;
                    worldTriangleIndices = clipping.ClippedTriangles.Items;
                    worldTriangleIndicesLength = clipping.ClippedTriangles.Count;
                    uvs = clipping.ClippedUVs.Items;
                }

                var textureSizeX = texture.Size.X;
                var textureSizeY = texture.Size.Y;

                SFML.Graphics.Vertex vertex = new();
                vertex.Color.R = (byte)(tintR * 255);
                vertex.Color.G = (byte)(tintG * 255);
                vertex.Color.B = (byte)(tintB * 255);
                vertex.Color.A = (byte)(tintA * 255);

                // 必须用 worldTriangleIndicesLength 不能直接 foreach
                for (int i = 0; i < worldTriangleIndicesLength; i++)
                {
                    var index = worldTriangleIndices[i] * 2;
                    vertex.Position.X = worldVertices[index];
                    vertex.Position.Y = worldVertices[index + 1];
                    vertex.TexCoords.X = uvs[index] * textureSizeX;
                    vertex.TexCoords.Y = uvs[index + 1] * textureSizeY;
                    vertexArray.Append(vertex);
                }

                clipping.ClipEnd(slot);
            }

            if (UsePremultipliedAlpha && (states.BlendMode == SFMLBlendMode.Normal || states.BlendMode == SFMLBlendMode.Additive))
                states.Shader = premultipliedAlphaFragmentShader;
            else
                states.Shader = null;
            target.Draw(vertexArray, states);
            clipping.ClipEnd();
        }
    }
}
