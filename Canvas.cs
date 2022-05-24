using System;
using System.Collections.Generic;
using System.Text;
using ButtonUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity_Simulation
{ 
    public class Canvas : UIObject
    {
        readonly List<Body> bodies = new List<Body>();
        readonly float gravConstant = 100;
        Vector2 centerOfMass = new Vector2();
        public bool trajectories = false;
        List<Point> trajectoryPoints = new List<Point>();
        public Color trajectoryColor = Color.Red;
        public float speed = 1f;

        public Canvas(Rectangle _rect) : base(_rect)
        {
            Texture2D texture = Game1.self.Content.Load<Texture2D>("body1");
            bodies.Add(new Body(new Vector2(800, 500), new Vector2(), 20, 50, texture));
            bodies.Add(new Body(new Vector2(800, 800), new Vector2(12, 0), 30, 100, texture));
            bodies.Add(new Body(new Vector2(800, 850), new Vector2(16, 0), 5, 1, texture));
            bodies.Add(new Body(new Vector2(400, 400), new Vector2(), 50, 500, texture));
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
                //create body
            }

            //gravity acceleration
            foreach (Body body1 in bodies)
            {
                foreach (Body body2 in bodies)
                {
                    if (body1 != body2)
                    {
                        Accelerate(body1, body2);
                    }
                }
            }

            //move bodies
            foreach (Body body in bodies)
            {
                body.Update(speed);
            }

            //check for collisions
            HashSet<Body> toBeRemoved = new HashSet<Body>();
            foreach (Body body1 in bodies)
            {
                foreach (Body body2 in bodies)
                {
                    if ((body1 != body2) && !toBeRemoved.Contains(body1) && !toBeRemoved.Contains(body2) && body1.OverlapsWith(body2))
                    {
                        if (body1.mass < body2.mass)
                        {                            
                            body2.CollidesWith(body1);
                            toBeRemoved.Add(body1);
                            
                        }
                        else
                        {
                            body1.CollidesWith(body2);
                            toBeRemoved.Add(body2);
                        }
                    }
                }
            }
            foreach (Body body in toBeRemoved)
            {
                bodies.Remove(body);
            }         
        }

        private void UpdateCenterOfMass()
        {
            Body centerBody = new Body(bodies[0].pos, bodies[0].velocity, 100, bodies[0].mass, bodies[0].texture);
            for (int i = 1; i < bodies.Count; i++)
            {
                centerBody.CollidesWith(bodies[i]);
            }
            centerOfMass = centerBody.pos;
        }

        public void TrackCenterOfMass()
        {
            UpdateCenterOfMass();
            Vector2 distance = rect.Center.ToVector2() - centerOfMass;
            foreach (Body body in bodies)
            {
                body.pos += distance;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //draw background
            DrawShape.Rectangle(spriteBatch, rect, Color.Yellow, 10, Color.Black);

            //draw trajectories
            if (trajectories)
            {
                foreach (Body body in bodies)
                {
                    if (!trajectoryPoints.Contains(body.pos.ToPoint())) trajectoryPoints.Add(body.pos.ToPoint());
                }

                foreach (Point point in trajectoryPoints)
                {
                    DrawShape.Rectangle(spriteBatch, new Rectangle(point.X, point.Y, 2, 2), trajectoryColor);
                }
            }
            else trajectoryPoints.Clear();

            //draw bodies
            foreach (Body body in bodies)
            {
                if (rect.Intersects(body.drawbox)) body.Draw(spriteBatch);
            }
        }
        #endregion

        private void Accelerate(Body sourceBody, Body targetBody)
        {
            Vector2 direction = sourceBody.pos - targetBody.pos;
            float distance = direction.Length();
            direction.Normalize();
            float magnitude = gravConstant * sourceBody.mass / (float)Math.Pow(distance, 2);

            targetBody.acceleration += direction * magnitude;
        }
    }
}
