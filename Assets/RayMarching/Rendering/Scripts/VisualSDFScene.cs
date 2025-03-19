using System;
using System.Collections.Generic;

namespace RayMarching.Rendering
{
    public class VisualSDFScene : IScene<IVisualSDFObject>
    {
        private readonly List<IVisualSDFObject> m_objects;

        public IReadOnlyCollection<IVisualSDFObject> Objects => m_objects;

        public VisualSDFScene()
        {
            m_objects = new List<IVisualSDFObject>();
        }

        public VisualSDFScene(IEnumerable<IVisualSDFObject> objects)
        {
            m_objects = new List<IVisualSDFObject>(objects);
        }

        public void AddObject(IVisualSDFObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (m_objects.Contains(obj))
                throw new ArgumentException($"{obj} has already contains in the scene");

            m_objects.Add(obj);
        }

        public bool RemoveObject(IVisualSDFObject obj)
        {
            return m_objects.Remove(obj);
        }

        public void ApplyContext(ISDFRendererContext context)
        {
            foreach (IVisualSDFObject obj in m_objects)
            {
                EnableKeywords(context, obj.ShaderKeywords);
                obj.ApplyContext(context);
            }
        }

        public void ApplyContext(ISDFRendererContext context, ref List<string> enabledKeywords)
        {
            if (enabledKeywords == null)
                enabledKeywords = new List<string>();
            else
                enabledKeywords.Clear();

            foreach (IVisualSDFObject obj in m_objects)
            {
                IReadOnlyCollection<string> objKeywords = obj.ShaderKeywords;

                enabledKeywords.AddRange(objKeywords);

                EnableKeywords(context, objKeywords);
                obj.ApplyContext(context);
            }
        }

        private static void EnableKeywords(ISDFRendererContext context, IEnumerable<string> keywords)
        {
            foreach (string keyword in keywords)
            {
                context.EnableKeyword(keyword);
            }
        }
    }
}