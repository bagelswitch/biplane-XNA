using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Biplane.Components
{
    class DebugLineManager : DrawableGameComponent {

        private static List<DebugLineVert> debugLines_ = new List<DebugLineVert>();

        private GraphicsDevice dev_;
        private ContentManager content_;

        public DebugLineManager(Game game)
            : base(game) {
            content_ = game.Content;
            dev_ = Game.GraphicsDevice;
        }

        public static void DebugLine(Vector3 p, Vector3 d)
        {
            DebugLine(p, d, 0xffffffff);
        }

        public static void DebugLine(Vector3 p, Vector3 d, uint c)
        {
            if (debugLines_.Count >= MAX_DEBUG_LINES * 2)
            {
                return;
            }
            DebugLineVert dlv = new DebugLineVert();
            dlv.pos = p;
            dlv.color = c;
            debugLines_.Add(dlv);
            dlv.pos = d;
            dlv.color = c;
            debugLines_.Add(dlv);
        }

        VertexBuffer debugLineVb_ = null;
        VertexDeclaration debugLineVdecl_;
        Effect debugLineFx_;
        EffectParameter debugLineWvp_;
        const int MAX_DEBUG_LINES = 1024;

        struct DebugLineVert
        {
            public Vector3 pos;
            public uint color;
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            dev_ = Game.GraphicsDevice;

            debugLineVb_ = new VertexBuffer(dev_, MAX_DEBUG_LINES * 16 * 2,
                BufferUsage.WriteOnly);

            VertexElement[] ve = new VertexElement[2];
            ve[0].Offset = 0;
            ve[0].VertexElementFormat = VertexElementFormat.Vector3;
            ve[0].VertexElementUsage = VertexElementUsage.Position;
            ve[1].Offset = 12;
            ve[1].VertexElementFormat = VertexElementFormat.Color;
            ve[1].VertexElementUsage = VertexElementUsage.Color;
            debugLineVdecl_ = new VertexDeclaration(dev_, ve);
            debugLineFx_ = content_.Load<Effect>("Effects\\Lines");
            debugLineWvp_ = debugLineFx_.Parameters["g_mWorldViewProjection"];
        }

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            debugLineVdecl_.Dispose();
            debugLineVdecl_ = null;
            debugLineVb_.Dispose();
            debugLineVb_ = null;
            debugLineFx_.Dispose();
            debugLineFx_ = null;
            debugLines_.Clear();
        }

        public static void ClearDebugLines()
        {
            debugLines_.Clear();
        }

        public override void Draw(GameTime gameTime)
        {
            BiplaneGame game = ((BiplaneGame)this.Game);

            if (debugLines_.Count > 0)
            {
                debugLineVb_.SetData<DebugLineVert>(debugLines_.ToArray());
                dev_.Vertices[0].SetSource(debugLineVb_, 0, 16);
                dev_.VertexDeclaration = debugLineVdecl_;
                debugLineWvp_.SetValue(game.gameState.viewMatrix * game.gameState.projectionMatrix);
                debugLineFx_.Begin(SaveStateMode.None);
                debugLineFx_.CurrentTechnique.Passes[0].Begin();
                dev_.DrawPrimitives(PrimitiveType.LineList, 0, debugLines_.Count / 2);
                debugLineFx_.CurrentTechnique.Passes[0].End();
                debugLineFx_.End();
            }
            ClearDebugLines();
        }
    }
}
