using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LD27
{
    class Player : Sprite
    {
        const string assetName = "Images/player";

        public Level CurrentLevel { get; set; }

        private const int w = 24;
        private const int h = 24;

        public bool InTrap { get; set; }

        float radius;
        Vector2 center;
        int angle = 0;

        private Vector2 acceleration = Vector2.Zero;
        private Vector2 velocity = new Vector2(2f, 2f);

        float accelerationY = -45f;

        public bool Freeze { get; set; }
        
        public Player()
        {
            this.InTrap = false;
            this.Color = Color.White;
            this.Position = new Vector2(480 / 2 - w / 2, 750); //yay magic numbers
        }

        public void Initialize()
        {
            this.InTrap = false;
            this.Position = new Vector2(480 / 2 - w / 2, 750);
            acceleration = Vector2.Zero;
        }

        public void LoadContent(ContentManager theContentManager)
        {
            base.LoadContent(theContentManager, assetName);
        }

        public void Update(GameTime theGameTime, KeyboardState currentKeyboardState, KeyboardState oldKeyboardState)
        {
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                acceleration.X -= 150f * (float)theGameTime.ElapsedGameTime.TotalSeconds;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                acceleration.X += 150f * (float)theGameTime.ElapsedGameTime.TotalSeconds;
            }

            if (!InTrap)
            {
                if (acceleration.X < 0)
                {
                    if (!CollisionHandler.IsLeftHorizontalCollision(new Rectangle((int)this.X - 10, (int)this.Y - 5, 20, 10), CurrentLevel.LevelObstacles))
                    {
                        this.Position.X += velocity.X * acceleration.X * (float)theGameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
                else
                {
                    if (!CollisionHandler.IsRightHorizontalCollision(new Rectangle((int)this.X - 10, (int)this.Y - 5, 20, 10), CurrentLevel.LevelObstacles))
                    {
                        this.Position.X += velocity.X * acceleration.X * (float)theGameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
                if (!CollisionHandler.IsVerticalCollision(new Rectangle((int)this.X - 5, (int)this.Y - 10, 10, 20), CurrentLevel.LevelObstacles))
                {
                    this.Position.Y += velocity.Y * accelerationY * (float)theGameTime.ElapsedGameTime.TotalSeconds;
                }

                center = CollisionHandler.TrapCollisions(this.Position, CurrentLevel.LevelTraps);
                if (center != Vector2.Zero)
                {
                    radius = (this.Position - center).Length();
                    InTrap = true;
                }

                if (X < 11)
                    X = 11;
                if (X > 480 - 11)
                    X = 480 - 11;
            }
            else
            {
                Fall();
            }
        }

        private void Fall()
        {
            if (radius > 0)
            {
                this.X = radius * Fcos(angle * FPI / 180f) + center.X;
                this.Y = radius * Fsin(angle * FPI / 180f) + center.Y;
            }

            radius *= 0.975f;
            angle += 20;
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            base.Draw(theSpriteBatch, new Vector2(this.texture.Width / 2, this.texture.Height / 2), this.Position, this.Color, this.Rotation);
        }
    }
}
