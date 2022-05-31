﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using ButtonUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity_Simulation
{ 
    public class Canvas : UIObject
    {
        List<Body> bodies = new List<Body>();
        Vector2 centerOfMass = new Vector2();
        public bool trajectories = false, tracking = true;
        List<Point> trajectoryPoints = new List<Point>();
        Color trajectoryColor = Color.Red;
        public Texture2D sky;

        public Canvas(Rectangle _rect) : base(_rect)
        {
            Texture2D texture = Game1.self.Content.Load<Texture2D>("body1");
            //bodies.Add(new Body(new Vector2(800, 500), new Vector2(), 5, 50, texture));
            Physics.CreateBody(new Body(new Vector2(800, 800), new Vector2(12, 0), 10, 100, texture));
            //bodies.Add(new Body(new Vector2(800, 850), new Vector2(16, 0), 5, 1, texture));
            Physics.CreateBody(new Body(new Vector2(400, 400), new Vector2(), 50, 500, texture));
        }

        #region cycle
        public override void Update(MouseState mouse, KeyboardState keyboard)
        {
            Update(mouse);
        }

        public void Update(MouseState mouse)
        {
            if ((mouse.LeftButton == ButtonState.Pressed) && rect.Contains(mouse.Position))
            {
                //Body body = new Body(new Vector2(), new Vector2(), 0, 0, );
                //Physics.CreateBody(body);
            }

            bodies = Physics.bodyStates;

            //track movement
            if (tracking)
            {
                UpdateCenterOfMass();
                TrackCenterOfMass();
            }            
            
            //draw trajectories
            if (trajectories) MarkTrajectories();   
            

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //draw background
            //Rectangle background = new Rectangle(centerOfMass.ToPoint() - rect.Center, rect.Size);
            Rectangle background = rect;
            spriteBatch.Draw(sky, background, Color.White);

            //draw trajectories
            if (trajectories)
            {
                foreach (Point point in trajectoryPoints)
                {
                    DrawShape.Rectangle(spriteBatch, new Rectangle(point.X - 1, point.Y - 1, 2, 2), trajectoryColor);
                }
            }
            else trajectoryPoints.Clear();

            //draw bodies
            foreach (Body body in bodies)
            {
                body.UpdateTexture();
                if (rect.Intersects(body.drawbox)) body.Draw(spriteBatch);
            }
        }
        #endregion
        private void UpdateCenterOfMass()
        {
            if (bodies.Count > 0)
            {
                Body centerBody = new Body(bodies[0].pos, bodies[0].velocity, 100, bodies[0].mass, bodies[0].texture);
                for (int i = 1; i < bodies.Count; i++)
                {
                    centerBody.CollidesWith(bodies[i]);
                }
                centerOfMass = centerBody.pos;
            }
            
        }

        public void TrackCenterOfMass()
        {
            Vector2 distance = rect.Center.ToVector2() - centerOfMass;
            foreach (Body body in bodies)
            {
                body.pos += distance;
            }
        }
        private void MarkTrajectories()
        {
            foreach (Body body in bodies)
            {
                Point pos = body.pos.ToPoint();
                if (!trajectoryPoints.Contains(pos)) trajectoryPoints.Add(pos);
            }
        }
    }
}
