using RayMarching.Physics;
using RayMarching.Rendering;
using System;
using System.Collections.Generic;

namespace RayMarching
{
    public class SDFScene : IScene<ISDFObject>
    {
        private VisualSDFScene m_visualPart;
        private PhysicsSDFScene m_physicsPart;

        public VisualSDFScene VisualPart
        {
            get => m_visualPart;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                m_visualPart = value;
            }
        }

        public PhysicsSDFScene PhysicsPart
        {
            get => m_physicsPart;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                m_physicsPart = value;
            }
        }

        public IReadOnlyCollection<ISDFObject> Objects => throw new NotSupportedException();

        public SDFScene()
        {
            m_visualPart = new VisualSDFScene();
            m_physicsPart = new PhysicsSDFScene();
        }

        public SDFScene(VisualSDFScene visualPart, PhysicsSDFScene physicsPart)
        {
            if (visualPart == null)
                throw new ArgumentNullException(nameof(visualPart));

            if (physicsPart == null)
                throw new ArgumentNullException(nameof(physicsPart));

            m_visualPart = visualPart;
            m_physicsPart = physicsPart;
        }

        public SDFScene(IEnumerable<IVisualSDFObject> visualObjects, IEnumerable<IPhysicsSDFObject> physicsObjects)
        {
            m_visualPart = new VisualSDFScene(visualObjects);
            m_physicsPart = new PhysicsSDFScene(physicsObjects);
        }

        public void AddObject(ISDFObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            m_visualPart.AddObject(obj);
            m_physicsPart.AddObject(obj);
        }

        public bool RemoveObject(ISDFObject obj)
        {
            return m_visualPart.RemoveObject(obj) || m_physicsPart.RemoveObject(obj);
        }
    }
}