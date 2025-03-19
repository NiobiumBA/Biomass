using RayMarching.Physics;
using RayMarching.Rendering;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RayMarching
{
    public static class SDFSceneManager
    {
        private static SDFScene s_activeScene;

        public static SDFScene ActiveScene
        {
            get
            {
                return s_activeScene;
            }
            set
            {
                s_activeScene = value;
            }
        }

        public static SDFScene CreateFromUnityScene(Scene scene)
        {
            List<IVisualSDFObject> visualObjects = new();
            List<IPhysicsSDFObject> physicsObjects = new();

            foreach (GameObject rootObj in scene.GetRootGameObjects())
            {
                if (rootObj.activeSelf == false)
                    continue;

                foreach (SDFObjectPresenter presenter in rootObj.GetComponentsInChildren<SDFObjectPresenter>())
                {
                    if (presenter.isActiveAndEnabled == false)
                        continue;

                    if (presenter.VisualObject != null)
                        visualObjects.Add(presenter.VisualObject);

                    if (presenter.PhysicsObject != null)
                        physicsObjects.Add(presenter.PhysicsObject);
                }
            }

            return new SDFScene(visualObjects, physicsObjects);
        }
    }
}