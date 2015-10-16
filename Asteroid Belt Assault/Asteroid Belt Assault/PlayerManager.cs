using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroid_Belt_Assault
{
    class PlayerManager
    {
        public Sprite playerDirection;
        public Sprite playerSpriteDown;
        public Sprite playerSpriteLeft;
        public Sprite playerSpriteUp;
        public Sprite playerSpriteRight;
        public Sprite fireball;
        private float playerSpeed = 160.0f;
        private Rectangle playerAreaLimit;

        public long PlayerScore = 0;
        public int LivesRemaining = 3;
        public bool Destroyed = false;

        private Vector2 gunOffset = new Vector2(25, 10);
        private float shotTimer = 0.0f;
        private float minShotTimer = 0.2f;
        private int playerRadius = 15;
        public ShotManager PlayerShotManager;

        public PlayerManager(
            Texture2D texture,  
            Rectangle initialFrame,
            int frameCount,
            Rectangle screenBounds)
        {
            playerSpriteDown = new Sprite(
                new Vector2(500, 500),
                texture,
                initialFrame,
                Vector2.Zero);
            playerSpriteLeft = new Sprite(
                new Vector2(500, 500),
                texture,
                new Rectangle(1, 128, 63, 125),
                Vector2.Zero);
            playerSpriteUp = new Sprite(
                new Vector2(500, 500),
                texture,
                new Rectangle(1, 254, 63, 125),
                Vector2.Zero);
            playerSpriteRight = new Sprite(
                new Vector2(500, 500),
                texture,
                new Rectangle(1, 380, 63, 125),
                Vector2.Zero);
            playerDirection = playerSpriteDown;

            PlayerShotManager = new ShotManager(
                texture,
                new Rectangle(0, 300, 5, 5),
                4,
                2,
                250f,
                screenBounds);

            playerAreaLimit =
                new Rectangle(
                    0,
                    0,
                    screenBounds.Width,
                    screenBounds.Height);

            for (int x = 1; x < frameCount; x++)
            {
                playerSpriteDown.AddFrame(
                    new Rectangle(
                        initialFrame.X + (initialFrame.Width * x),
                        initialFrame.Y,
                        initialFrame.Width,
                        initialFrame.Height));
            }
            for (int x = 1; x < frameCount; x++)
            {
                playerSpriteLeft.AddFrame(
                    new Rectangle(
                        1 + (63 * x),
                        128,
                        63,
                        127));
            }
            for (int x = 1; x < frameCount; x++)
            {
                playerSpriteUp.AddFrame(
                    new Rectangle(
                        1 + (63 * x),
                        254,
                        63,
                        125));
            }
            for (int x = 1; x < frameCount; x++)
            {
                playerSpriteRight.AddFrame(
                    new Rectangle(
                        1 + (63 * x),
                        380,
                        63,
                        127));
            }
            playerSpriteDown.CollisionRadius = playerRadius;
            playerSpriteLeft.CollisionRadius = playerRadius;
            playerSpriteUp.CollisionRadius = playerRadius;
            playerSpriteRight.CollisionRadius = playerRadius;

        }

        private void FireShot()
        {
            if (shotTimer >= minShotTimer)
            {
                PlayerShotManager.FireShot(
                    playerDirection.Location + gunOffset,
                    new Vector2(0, -1),
                    true);
                shotTimer = 0.0f;
            }
        }

        private void HandleKeyboardInput(KeyboardState keyState)
        {
            if (keyState.IsKeyDown(Keys.Up))
            {
                playerSpriteUp.Moving = true;
                playerDirection = playerSpriteUp;
                playerDirection.Velocity += new Vector2(0, -1);
            }else if (keyState.IsKeyDown(Keys.Down))
            {
                playerSpriteDown.Moving = true;
                playerDirection = playerSpriteDown;
                playerDirection.Velocity += new Vector2(0, 1);
            }else if (keyState.IsKeyDown(Keys.Left))
            {
                playerSpriteLeft.Moving = true;
                playerDirection = playerSpriteLeft;
                playerDirection.Velocity += new Vector2(-1, 0);
            }else if (keyState.IsKeyDown(Keys.Right))
            {
                playerSpriteRight.Moving = true;
                playerDirection = playerSpriteRight;
                playerDirection.Velocity += new Vector2(1, 0);
            }else
            {
                playerSpriteDown.Moving = false;
                playerSpriteLeft.Moving = false;
                playerSpriteUp.Moving = false;
                playerSpriteRight.Moving = false;
            }

            if (keyState.IsKeyDown(Keys.Space))
            {
                FireShot();
            }
        }

        private void HandleGamepadInput(GamePadState gamePadState)
        {
            Vector2 direction = new Vector2(
                    gamePadState.ThumbSticks.Left.X,
                    -gamePadState.ThumbSticks.Left.Y);
            // Dead zone cancelation
            if (Math.Abs(direction.X) < .1)
                direction.X = 0;
            if (Math.Abs(direction.Y) < .1)
                direction.Y = 0;

            //normalize and cardinalize direction
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
                direction.Y = 0;
            else
                direction.X = 0;
            if (direction.Length() != 0f)
                direction.Normalize();

            if (direction.Y == -1)
            {
                playerSpriteUp.Moving = true;
                playerDirection = playerSpriteUp;
                playerDirection.Velocity += new Vector2(0, -1);
            }else if (direction.Y == 1)
            {
                playerSpriteDown.Moving = true;
                playerDirection = playerSpriteDown;
                playerDirection.Velocity += new Vector2(0, 1);
            }else if (direction.X == -1)
            {
                playerSpriteLeft.Moving = true;
                playerDirection = playerSpriteLeft;
                playerDirection.Velocity += new Vector2(-1, 0);
            }else if (direction.X == 1)
            {
                playerSpriteRight.Moving = true;
                playerDirection = playerSpriteRight;
                playerDirection.Velocity += new Vector2(1, 0);
            }

            if (gamePadState.Buttons.A == ButtonState.Pressed)
            {
                FireShot();
            }
        }

        private void imposeMovementLimits()
        {
            Vector2 location = playerDirection.Location;

            if (location.X < playerAreaLimit.X)
                location.X = playerAreaLimit.X;

            if (location.X >
                (playerAreaLimit.Right - playerDirection.Source.Width))
                location.X =
                    (playerAreaLimit.Right - playerDirection.Source.Width);

            if (location.Y < playerAreaLimit.Y)
                location.Y = playerAreaLimit.Y;

            if (location.Y >
                (playerAreaLimit.Bottom - playerDirection.Source.Height))
                location.Y =
                    (playerAreaLimit.Bottom - playerDirection.Source.Height);

            playerDirection.Location = location;
        }

        public void Update(GameTime gameTime)
        {
            PlayerShotManager.Update(gameTime);

            if (!Destroyed)
            {
                playerDirection.Velocity = Vector2.Zero;
                playerSpriteDown.Location = playerDirection.Location;
                playerSpriteLeft.Location = playerDirection.Location;
                playerSpriteUp.Location = playerDirection.Location;
                playerSpriteRight.Location = playerDirection.Location;


                shotTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                HandleKeyboardInput(Keyboard.GetState());
                HandleGamepadInput(GamePad.GetState(PlayerIndex.One));

                playerDirection.Velocity.Normalize();
                playerDirection.Velocity *= playerSpeed;

                playerDirection.Update(gameTime);
                imposeMovementLimits();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerShotManager.Draw(spriteBatch);

            if (!Destroyed)
            {
                playerDirection.Draw(spriteBatch);
            }
        }

    }
}
