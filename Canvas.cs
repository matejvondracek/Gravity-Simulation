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
        public Canvas(Rectangle _rect) : base(_rect)
        {
            Texture2D texture = Game1.self.Content.Load<Texture2D>("body1");
            bodies.Add(new Body(new Vector2(800, 500), new Vector2(), 50, 500, texture));
            bodies.Add(new Body(new Vector2(800, 800), new Vector2(12, 0), 10, 10, texture));
            bodies.Add(new Body(new Vector2(800, 850), new Vector2(16, 0), 5, 1, texture));
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
                body.Update();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Body body in bodies)
            {
                body.Draw(spriteBatch);
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
