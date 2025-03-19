using RayMarching.Physics;
using RayMarching.Rendering;
using UnityEngine;

namespace RayMarching
{
    public abstract class SDFObjectPresenter : MonoBehaviour
    {
        private IVisualSDFObject m_addedVisualObject;
        private IPhysicsSDFObject m_addedPhysicsObject;

        public abstract IVisualSDFObject VisualObject { get; }
        public abstract IPhysicsSDFObject PhysicsObject { get; }

        protected virtual void OnEnable()
        {
            AddObjectToActiveScene();
        }

        protected virtual void OnDisable()
        {
            RemoveObjectFromActiveScene();
        }

        protected void UpdateActiveScene()
        {
            if (isActiveAndEnabled == false)
                return;

            RemoveObjectFromActiveScene();
            AddObjectToActiveScene();
        }

        protected void AddObjectToActiveScene()
        {
            if (SDFSceneManager.ActiveScene == null)
                SDFSceneManager.ActiveScene = new SDFScene();

            if (VisualObject != null)
            {
                m_addedVisualObject = VisualObject;
                SDFSceneManager.ActiveScene.VisualPart.AddObject(m_addedVisualObject);
            }

            if (PhysicsObject != null)
            {
                m_addedPhysicsObject = PhysicsObject;
                SDFSceneManager.ActiveScene.PhysicsPart.AddObject(m_addedPhysicsObject);
            }
        }

        protected void RemoveObjectFromActiveScene()
        {
            if (SDFSceneManager.ActiveScene == null)
                return;

            SDFSceneManager.ActiveScene.VisualPart.RemoveObject(m_addedVisualObject);
            SDFSceneManager.ActiveScene.PhysicsPart.RemoveObject(m_addedPhysicsObject);

            m_addedVisualObject = null;
            m_addedPhysicsObject = null;
        }
    }
}