using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ButtonUI;
using DecimalMath;
using DecimalVector2Project;

namespace Gravity_Simulation
{   
    public class Body
    {
        public Texture2D texture;
        public Rectangle drawbox;
        public DecimalVector2 pos, velocity, acceleration = new DecimalVector2(), momentum;
        public decimal mass, radius;
        public Label label;
        public SpriteFont font;


        public Body(DecimalVector2 _pos, DecimalVector2 _velocity, decimal _radius, decimal _mass, Texture2D _texture)
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
            return (DecimalEx.Pow(DecimalEx.Pow(this.pos.X - body.pos.X, 2) + DecimalEx.Pow(this.pos.Y - body.pos.Y, 2), 1 / 2m) <= this.radius + body.radius);
        }

        public void CollidesWith(Body body)
        {                      
            DecimalVector2 distance = body.pos - pos;
            pos += distance / (mass + body.mass) * body.mass;

            mass += body.mass;
            momentum += body.momentum;
            velocity = momentum / mass;

            decimal volume1 = 4m / 3m * DecimalEx.Pi * DecimalEx.Pow(radius, 3);
            decimal volume2 = 4m / 3m * DecimalEx.Pi * DecimalEx.Pow(body.radius, 3);
            radius = DecimalEx.Pow(3m * (volume1 + volume2) / (4m * DecimalEx.Pi), 1m / 3m);
        }

        public DecimalVector2 GetMomementum()
        {
            momentum = velocity * mass;
            return momentum;
        }

        #region cycle
        public void Update(decimal precision)
        {
            UpdateTexture();

            velocity += acceleration / precision;
            pos += velocity / precision;      

            acceleration = new DecimalVector2();
            GetMomementum();
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
            drawbox = new Rectangle((int)(pos.X - radius), (int)(pos.Y - radius), (int)(2 * radius), (int)(2 * radius));

            //update label
            Rectangle labelRect = new Rectangle(drawbox.X + (int)radius / 2, drawbox.Y + (int)radius / 2, (int)radius, (int)radius);
            label = new Label(labelRect, Convert.ToString(mass), font, Color.Black);
        }
    }
}
