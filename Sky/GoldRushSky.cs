using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace GoldRush.Sky
{
    public class GoldRushSky : CustomSky
	{
		private bool isActive = true;
		private float intensity;

		public override void Update(GameTime gameTime) 
		{
            if (isActive)
            {
				intensity += 0.01f;
            }
            else    
            {
				intensity -= 0.01f;
            }
			intensity = Utils.Clamp(intensity, 0f, 1f);

		}



		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (maxDepth >= 0 && minDepth < 0)
			{
				DrawCurtain(spriteBatch, 0, intensity, Color.White);
				if (intensity > 0 && intensity < 1)
                {
					for (float i = 0; i <= 10f; i++)
                    {
						float k = i / 300f;
						float a = (10f - i) / 10f;
						DrawCurtain(spriteBatch, intensity + k, intensity + k + 0.01f, Color.White * a);
                    }
                }
			}
		}

		private void DrawCurtain(SpriteBatch sb, float d1, float d2, Color color)
		{
			Texture2D tex = ModContent.Request<Texture2D>("GoldRush/Sky/GoldRushSky").Value;
			sb.Draw(tex, new Rectangle((int)(Main.screenWidth * d1), 0, (int)(Main.screenWidth * d2), Main.screenHeight), new Rectangle((int)(tex.Width * d1), 0, (int)(tex.Width * d2), tex.Height), color);
		}

		public override float GetCloudAlpha() 
		{
			return 0f;
		}

		public override void Activate(Vector2 position, params object[] args) 
		{
			isActive = true;
		}

		public override void Deactivate(params object[] args) 
		{
			isActive = false;
		}

		public override void Reset() 
		{
			isActive = false;
		}

		public override bool IsActive() 
		{
			return isActive || intensity > 0f;
		}
	}
}