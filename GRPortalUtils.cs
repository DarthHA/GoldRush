using GoldRush.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace GoldRush
{
    public static class GRPortalUtils
    {

        /// <summary>
        /// ��ô��������ı��
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static float GetMaxID(int owner)
        {
            float max = -1;
            foreach (Projectile p in Main.projectile)
            {
                if (p.owner == owner && p.active && p.type == ModContent.ProjectileType<GoldPortal>())
                {
                    if (p.ai[1] == 0)
                    {
                        if (p.ai[0] > max)
                        {
                            max = p.ai[0];

                        }
                    }
                }
            }
            return max;
        }


        /// <summary>
        /// ��������Ŀ
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static int PortalCount(int owner)
        {
            int result = 0;
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.owner == owner && proj.type == ModContent.ProjectileType<GoldPortal>())
                {
                    if (proj.ai[1] == 0)
                    {
                        result++;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// ���ָ����Ŵ�����
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int GetPortal(int owner, int id)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.owner == owner && proj.type == ModContent.ProjectileType<GoldPortal>())
                {
                    if (proj.ai[0] == id && proj.ai[1] == 0)
                    {
                        return proj.whoAmI;
                    }
                }
            }
            return -1;
        }


        /// <summary>
        /// ���������д����ű��ǰ��
        /// </summary>
        /// <param name="owner"></param>
        public static void PullProjID(int owner)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.owner == owner && proj.type == ModContent.ProjectileType<GoldPortal>())
                {
                    if (proj.ai[1] == 0)
                    {
                        proj.ai[0]--;
                    }
                }
            }
        }

        /// <summary>
        /// ��ô������������
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Vector2 GetPortalEnter(int owner, int id)
        {
            if (GetPortal(owner, id) == -1)
            {
                return new Vector2(0, 0);
            }
            else
            {
                Projectile portal = Main.projectile[GetPortal(owner, id)];
                return portal.Center - new Vector2(GoldPortal.PortalD / 2, 0) * portal.localAI[0];
            }
        }


        /// <summary>
        /// ��ô����ų�������
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Vector2 GetPortalOut(int owner, int id)
        {
            if (GetPortal(owner, id) == -1)
            {
                return new Vector2(0, 0);
            }
            else
            {
                Projectile portal = Main.projectile[GetPortal(owner, id)];
                return portal.Center + new Vector2(GoldPortal.PortalD / 2, 0) * portal.localAI[0];
            }
        }

        /// <summary>
        /// ���ָ����Ŵ�������������
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Vector2 GetPortalCenter(int owner, int id)
        {
            if (GetPortal(owner, id) == -1)
            {
                return new Vector2(0, 0);
            }
            else
            {
                Projectile portal = Main.projectile[GetPortal(owner, id)];
                return portal.Center;
            }
        }
    }
}