using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity_Simulation
{
    
    public class Body
    {
        public Texture2D texture;
        public Rectangle drawbox;
        public Vector2 pos, velocity, acceleration = new Vector2();
        public int radius, mass;


        public Body(Vector2 _pos, Vector2 _velocity, int _radius, int _mass, Texture2D _texture)
        {
            pos = _pos;
            velocity = _velocity;
            radius = _radius;
            mass = _mass;
            texture = _texture;
        }

        public bool CollidesWith(Body body)
        {
            return false;
        }

        #region cycle
        public void Update()
        {
            velocity += acceleration;
            pos += velocity;
            drawbox = new Rectangle((int)pos.X, (int)pos.Y, 2 * radius, 2 * radius);
            acceleration = new Vector2();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawbox, Color.White);
        }
        #endregion
    }
}
