using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ButtonUI;

namespace Gravity_Simulation
{
    
    public class Body
    {
        public Texture2D texture;
        public Rectangle drawbox;
        public Vector2 pos, velocity, acceleration = new Vector2(), momentum;
        public int radius, mass;
        public HashSet<Point> points = new HashSet<Point>();
        public Label label;
        public SpriteFont font;


        public Body(Vector2 _pos, Vector2 _velocity, int _radius, int _mass, Texture2D _texture)
        {
            pos = _pos;
            velocity = _velocity;
            radius = _radius;
            mass = _mass;
            texture = _texture;
            font = Game1.self.Content.Load<SpriteFont>("Arial");
            GetMomementum();
        }

        public bool OverlapsWith(Body body)
        {
            return points.Overlaps(body.points);
        }

        public void CollidesWith(Body body)
        {           
            
            Vector2 distance = body.pos - pos;
            pos += distance / (mass + body.mass) * body.mass;

            mass += body.mass;
            momentum = momentum + body.momentum;
            velocity = momentum / mass;

            int volume1 = (int)(4f / 3f * 3.14f * Math.Pow(radius, 3));
            int volume2 = (int)(4f / 3f * 3.14f * Math.Pow(body.radius, 3));
            radius = (int)Math.Cbrt((3f * (volume1 + volume2)) / (4f * 3.14f));
        }

        public Vector2 GetMomementum()
        {
            momentum = velocity * mass;
            return momentum;
        }

        #region cycle
        public void Update(float precision)
        {
            UpdateTexture();

            velocity += acceleration / precision;
            pos += velocity / precision;      

            drawbox = new Rectangle((int)pos.X - radius, (int)pos.Y - radius, 2 * radius, 2 * radius);
            acceleration = new Vector2();
            GetMomementum();

            //update set representation
            points.Clear();
            for (int x = -radius; x < radius; x++)
            {
                for (int y = -radius; y < radius; y++)
                {
                    Point point = new Point((int)pos.X + x, (int)pos.Y + y);
                    //now square, should be circle
                    points.Add(point);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawbox, Color.White);
            if (label != null) label.Draw(spriteBatch);
        }
        #endregion

        public void UpdateTexture()
        {
            //update drawbox
            drawbox = new Rectangle((int)pos.X - radius, (int)pos.Y - radius, 2 * radius, 2 * radius);

            //update label
            Rectangle labelRect = new Rectangle(drawbox.X + radius / 2, drawbox.Y + radius / 2, radius, radius);
            label = new Label(labelRect, Convert.ToString(mass), font, Color.Black);
        }
    }
}
