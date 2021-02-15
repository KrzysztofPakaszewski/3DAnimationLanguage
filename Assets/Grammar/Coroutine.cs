namespace anim {
    using UnityEngine;
    using System.Collections;

    public class Coroutine : MonoBehaviour{

        public static float delta = 0.05f;

        public Coroutine() { }

        public void handleAnimations(string action, GameObject target, Vector3 value, float time, Vector3? from, float start) {
            switch (action)
            {
                case "MOVE":
                    StartCoroutine(moveObject(target, value, time, from, start));
                    break;
                case "ROTATE":
                    StartCoroutine(rotateObject(target, value, time, from, start));
                    break;
                case "TRANSFORM":
                    StartCoroutine(scaleObject(target, value, time, from, start));
                    break;
                default:
                    throw new InvalidActionType();
            }
        }

        public IEnumerator moveObject(GameObject target, Vector3 to, float duration, Vector3? from, float start) {
            yield return new WaitForSeconds(start);
            Vector3 fromVal;
            if (from.HasValue)
            {
                fromVal = from.Value;
                target.transform.position = fromVal;
            }
            else {
                fromVal = target.transform.position;
            }
            Vector3 difference = to - fromVal;
            for (float curTime = 0 ; curTime < duration; curTime += delta) {
                float percent = curTime / duration;

                target.transform.position = fromVal + difference * percent;
                yield return new WaitForSeconds(delta);
            }
            target.transform.position = to;
        }

        public IEnumerator rotateObject(GameObject target, Vector3 to, float duration, Vector3? from, float start)
        {
            yield return new WaitForSeconds(start);
            Vector3 fromVal;
            if (from.HasValue)
            {
                fromVal = from.Value;
                target.transform.rotation = Quaternion.Euler(fromVal);
            }
            else
            {
                fromVal = target.transform.position;
            }
            Vector3 difference = to - fromVal;
            for (float curTime = 0; curTime < duration; curTime += delta)
            {
                float percent = curTime / duration;

                target.transform.rotation = Quaternion.Euler(fromVal + difference * percent);
                yield return new WaitForSeconds(delta);
            }
            target.transform.rotation = Quaternion.Euler(to);
        }

        public IEnumerator scaleObject(GameObject target, Vector3 to, float duration, Vector3? from, float start)
        {
            yield return new WaitForSeconds(start);
            Vector3 fromVal;
            if (from.HasValue)
            {
                fromVal = from.Value;
                target.transform.localScale = fromVal;
            }
            else
            {
                fromVal = target.transform.position;
            }
            Vector3 difference = to - fromVal;
            for (float curTime = 0; curTime < duration; curTime += delta)
            {
                float percent = curTime / duration;

                target.transform.localScale = fromVal + difference * percent;
                yield return new WaitForSeconds(delta);
            }
            target.transform.localScale = to;
        }
    }
}