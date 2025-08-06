using System.Collections.Generic;
using UnityEngine.Serialization;
using System.Linq;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("UI/Polygon", 12)]
    public class UIPoly : MaskableGraphic
    {
        [FormerlySerializedAs("m_Tex")]
        [SerializeField] Texture m_Texture;
        [SerializeField] Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

        public override Texture mainTexture
        {
            get
            {
                if (m_Texture == null)
                {
                    if (material != null && material.mainTexture != null)
                    {
                        return material.mainTexture;
                    }
                    return s_WhiteTexture;
                }

                return m_Texture;
            }
        }

        public Texture texture
        {
            get
            {
                return m_Texture;
            }
            set
            {
                if (m_Texture == value)
                    return;

                m_Texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        public Rect uvRect
        {
            get
            {
                return m_UVRect;
            }
            set
            {
                if (m_UVRect == value)
                    return;
                m_UVRect = value;
                SetVerticesDirty();
            }
        }

        public override void SetNativeSize()
        {
            Texture tex = mainTexture;
            if (tex != null)
            {
                int w = Mathf.RoundToInt(tex.width * uvRect.width);
                int h = Mathf.RoundToInt(tex.height * uvRect.height);
                rectTransform.anchorMax = rectTransform.anchorMin;
                rectTransform.sizeDelta = new Vector2(w, h);
            }
        }

        [SerializeField] 
        List<Vector2> m_Verticies = new List<Vector2>();
        public List<Vector2> m_Verts
        {
            get
            {
                return m_Verticies;
            }
            set
            {
                m_Verts = value;
            }
        }
        public List<Vector2> m_VertsWS
        {
            get
            {
                List<Vector2> verts = new();
                foreach (var v in m_Verticies)
                {
                    verts.Add(new Vector2(rectTransform.rect.center.x + rectTransform.rect.width * v.x, rectTransform.rect.center.y + rectTransform.rect.height * v.y));
                }
                return verts;
            }
            set
            {
                List<Vector2> verts = new();
                foreach (var v in value)
                {
                    verts.Add(new Vector2(v.x / rectTransform.rect.width - rectTransform.rect.center.x, v.y / rectTransform.rect.height - rectTransform.rect.center.y));
                }
                m_Verticies = verts;
            }
        }

        public List<Vector3Int> tris = new List<Vector3Int>();
        List<Vector2> uv => m_Verticies;
        public Vector3Int vector;
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            Texture tex = mainTexture;
            vh.Clear();
            if (tex != null)
            {
                List<Vector2> verts = m_VertsWS;

                tris = Triangles(verts.Count);
                for (int i = 0; i < verts.Count; i++)
                {
                    var color32 = color;
                    vh.AddVert(verts[i], color32, uv[i] + Vector2.one / 2);
                }

                for (int i = 0; i < tris.Count; i++)
                {
                    vh.AddTriangle(tris[i].x, tris[i].y, tris[i].z);
                }
            }
        }

        #region Polygon
        Vector3 Average(List<Vector3> vector3s)
        {
            int count = 0;
            Vector3 sum = Vector3.zero;
            foreach (Vector3 vert in vector3s)
            {
                sum += vert;
                count++;
            }
            Vector3 average = sum / (float)count;
            return average;
        }
        List<Vector3> AverageAdded(List<Vector3> vector3s)
        {
            List<Vector3> averageFirst = new List<Vector3>();
            averageFirst.Add(Average(vector3s));
            averageFirst.AddRange(vector3s);
            return averageFirst;
        }
        List<Vector3> OrderClockwise(List<Vector3> vector3s, Vector3 center)
        {
            vector3s = vector3s.OrderBy(i => Mathf.Atan2(center.z - i.z, center.x - i.x)).ToList();
            vector3s.Reverse();
            return vector3s;
        }

        List<Vector3Int> Triangles(int sides)
        {
            List<Vector3Int> tris = new();

            if (sides == 3)
            {
                tris.Add(new Vector3Int(0, 1, 2));
                return tris;
            }
            if (sides == 4)
            {
                tris.Add(new Vector3Int(0, 1, 2));
                tris.Add(new Vector3Int(0, 2, 3));
                return tris;
            }
            if (sides == 5)
            {
                tris.Add(new Vector3Int(0, 1, 2));
                tris.Add(new Vector3Int(0, 2, 3));
                tris.Add(new Vector3Int(0, 3, 4));
                return tris;
            }
            for (int i = 0; i < sides; i++)
            {
                Vector3Int tri = new();
                tri.x = 0;
                tri.y = i;
                if (i + 1 >= sides)
                {
                    tri.z = 1;
                }
                else
                {
                    tri.z = i + 1;
                }
                tris.Add(tri);
            }
            return tris;
        }

        List<Vector2> UV(List<Vector3> verticies)
        {
            List<Vector2> uvs = new List<Vector2>();
            foreach (Vector3 vert in verticies)
            {
                uvs.Add(new Vector2(vert.x, vert.y));
            }
            return uvs;
        }
        #endregion

        protected override void OnDidApplyAnimationProperties()
        {
            SetMaterialDirty();
            SetVerticesDirty();
            SetRaycastDirty();
        }
    }
}
