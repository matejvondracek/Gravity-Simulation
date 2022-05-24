﻿using System;
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
        }

        public Vector2 GetMomementum()
        {
            momentum = velocity * mass;
            return momentum;
        }

        #region cycle
        public void Update()
        {
            velocity += acceleration;
            pos += velocity;
            drawbox = new Rectangle((int)pos.X, (int)pos.Y, 2 * radius, 2 * radius);
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

            //update label
            Rectangle labelRect = new Rectangle(drawbox.X + radius / 2, drawbox.Y + radius / 2, radius, radius);
            label = new Label(labelRect, Convert.ToString(mass), font, Color.Yellow);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawbox, Color.White);
            if (label != null) label.Draw(spriteBatch);
        }
        #endregion
    }
}
