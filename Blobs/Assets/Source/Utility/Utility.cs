using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

#if !UNITY_EDITOR
[assembly: System.Reflection.AssemblyVersion("1.0.*")]
#endif

namespace Legend
{
    public static class Utility
    {
        public static Color32 DarkRed = new Color32(164, 0, 0, 255);
        public static Color32 LightBlue = new Color32(54, 124, 226, 255);
        public static Color32 Green = new Color32(67, 165, 26, 255);
        public static Color32 LightGreen = new Color32(194, 240, 145, 255);
        public static Color32 Purple = new Color32(224, 86, 253, 255);
        public static Color32 DarkGreen = new Color32(67, 165, 26, 255);
        public static Color32 LightRed = new Color32(225, 112, 85, 255);
        public static Color32 Red = new Color32(255, 0, 0, 255);
        public static Color32 Cyan = new Color32(0, 255, 255, 255);
        public static Color32 Magenta = new Color32(255, 0, 255, 255);
        public static Color32 Lime = new Color32(0, 255, 0, 255);
        public static Color32 Gold = new Color32(247, 215, 125, 255);
        public static Color32 DarkGold = new Color32(124, 111, 19, 255);
        //public const float UpdateInterval = 0.25f;

        public static System.Random Random = new System.Random();

        public static string Version { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        public static bool Flip()
        {
            // maxValue - The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to zero.
            return Random.Next(2) == 1;
        }

        public static int Roll4()
        {
            // maxValue - The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to zero.
            return Random.Next(4);
        }

        public static int Roll10()
        {
            // maxValue - The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to zero.
            return Random.Next(10) + 1;
        }

        internal static bool IsAnyKeyDown()
        {
            return Input.anyKeyDown
                || Input.touchCount > 0
                || Input.GetMouseButtonDown(0)
                || (Player.Instance != null && Player.Instance.Weapon.IsShooting);
        }

        internal static Vector3 FindEmptySpot(Vector3 position)
        {
            if (Physics2D.OverlapCircle(position, 0.01f, World.Instance.PickupPlacementCheckMask) == null)
                return position;

            var count = 0;
            while (count < 30)
            {
                var target = position + (Vector3)UnityEngine.Random.insideUnitCircle * 0.8f;
                if (Physics2D.OverlapCircle(target, 0.01f, World.Instance.PickupPlacementCheckMask) == null)
                    return target;
                count++;
            }
            //Debug.LogWarning("fdsa");
            return position;
        }

        public static bool IsEmptySpot(Vector3 position)
        {
            //Debug.DrawLine(position, Vector3.zero);
            if (Physics2D.OverlapCircle(position, 0.01f) == null)
            {
                //Debug.Log(position + " Is Empty");
                return true;
            }
            //Debug.Log(position + " Is NOT Empty");
            return false;
        }

        public static string FormatSeconds(double amount)
        {
            var minutes = Math.Floor(amount / 60);
            var seconds = Math.Floor(amount % 60);

            if (minutes > 0)
                return String.Format("{0:N0}:{1:00}", minutes, seconds);
            return String.Format("{1:N0}", minutes, seconds);
        }

        public static string AddSpaces(this string source)
        {
            return Regex.Replace(source, "(\\B[A-Z0-9])", " $1");
        }

        public static void DisableColliders(this GameObject source)
        {
            foreach (var collider in source.GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = false;
            }
        }

        public static void DisableColliders(this MonoBehaviour source)
        {
            foreach (var collider in source.GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = false;
            }
        }

        public static void EnableColliders(this GameObject source, bool includeTriggers = true)
        {
            foreach (var collider in source.GetComponentsInChildren<Collider2D>())
            {
                if (includeTriggers || !collider.isTrigger)
                    collider.enabled = true;
            }
        }

        public static void EnableColliders(this MonoBehaviour source, bool includeTriggers = true)
        {
            foreach (var collider in source.GetComponentsInChildren<Collider2D>())
            {
                if (includeTriggers || !collider.isTrigger)
                    collider.enabled = true;
            }
        }

        public static TSource RandomOrDefault<TSource>(this IList<TSource> source)
        {
            if (source.Count == 0)
                return default(TSource);

            return source[UnityEngine.Random.Range(0, source.Count)];
        }

        public static TSource RandomOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            var values = source.ToArray();

            if (values.Length == 0)
                return default(TSource);

            return values[UnityEngine.Random.Range(0, values.Length)];
        }

        //public static Coroutine DelayUntilAnimationEnd(this MonoBehaviour behaviour, Action action)
        //{
        //    var animator = behaviour.GetComponent<Animator>();
        //    //if (animator == null)
        //    //    return Delay(behaviour, action, 0);
        //    return Delay(behaviour, action, animator.GetCurrentAnimatorStateInfo(0).length);
        //}

        public static Coroutine Delay(this MonoBehaviour behaviour, Action action, float seconds)
        {
            if (seconds == 0)
            {
                action();
                return null;
            }
            else
                return behaviour.StartCoroutine(Delay(action, seconds));
        }

        public static IEnumerator Delay(Action action, float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            action();
        }

        static float playingUniqueUntil;
        public static AudioClip PlaySound(this Component behaviour, AudioClip clip, float volume = 1f, bool isUnique = false)
        {
            if (clip == null || behaviour == null || Level.IsHeadlessMode)
                return null;

            if (isUnique)
            {
                if (playingUniqueUntil > Time.time)
                    return null;
                playingUniqueUntil = Time.time + clip.length;
            }

            //Debug.Log("PlaySound " + clip.name);

            var source = behaviour.GetComponent<AudioSource>();
            if (source != null && source.enabled)
                source.PlayOneShot(clip, volume);
            else
            {
                AudioSource.PlayClipAtPoint(clip, behaviour.transform.position, volume);
            }

            return clip;
        }

        public static AudioClip PlaySound(this Component behaviour, IList<AudioClip> clips, float volume = 1f, bool isUnique = false)
        {
            if (clips != null && clips.Count > 0)
            {
                var clip = clips.RandomOrDefault();
                return behaviour.PlaySound(clip, volume, isUnique);
            }
            return null;
        }

        public static GameObject NetworkSpawn(this MonoBehaviour behaviour, GameObject prefab, Transform parent, Vector3 position = default(Vector3))
        {
            if (prefab == null)
                return null;

            var rotation = Quaternion.identity;
            if (behaviour != null)
            {
                rotation = behaviour.transform.rotation;
                if (position == default(Vector3))
                    position = behaviour.transform.position;
            }

            var result = GameObject.Instantiate(prefab, position, rotation, parent);
            NetworkServer.Spawn(result);
            return result;
        }

        public static GameObject Spawn(this MonoBehaviour behaviour, GameObject prefab, Vector3 position, Transform parent)
        {
            if (prefab == null)
                return null;

            var result = GameObject.Instantiate(prefab, position, behaviour.transform.rotation, parent);
            return result;
        }

        public static GameObject Spawn(this MonoBehaviour behaviour, GameObject prefab, Transform parent = null)
        {
            if (prefab == null)
                return null;

            var result = GameObject.Instantiate(prefab, behaviour.transform.position, behaviour.transform.rotation, parent);
            return result;
        }

        public static void DeactivateAtEndOfAnimation(this GameObject target, MonoBehaviour coroutineOwner)
        {
            var animator = target.GetComponent<Animator>();
            coroutineOwner.Delay(() => target.SetActive(false), animator.GetCurrentAnimatorStateInfo(0).length - .1f); // remove .1 here because animation blinks on dialog box
        }

        public static void DestroyObjectAtEndOfAnimation(this Animator animator)
        {
            GameObject.DestroyObject(animator.gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
        }

        public static bool HasClearPath(this Bounds source, Vector3 targetPosition, Transform excludeTarget = null)
        {
            var rayDirection = targetPosition - source.center;

            // add a skin to account for touching colliders.
            source.SetMinMax(source.min + new Vector3(0.01f, 0.01f, 0.01f), source.max - new Vector3(0.01f, 0.01f, 0.01f));

            //Debug.DrawRay(source.center, rayDirection, Color.green);
            //Debug.DrawLine(source.max, source.min, Color.blue);

            var original = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = false;
            var hit = Physics2D.BoxCast(source.center, source.size, Vector3.Angle(rayDirection, Vector3.forward), (Vector2)rayDirection, rayDirection.magnitude, World.Instance.PathFindingMask);
            Physics2D.queriesHitTriggers = original;

            if (hit.collider != null)
            {
                //Debug.Log("Hit " + hit.transform);

                if (excludeTarget == null)
                {
                    return true;
                }
                else
                {
                    var result = hit.transform == excludeTarget || hit.transform.IsChildOf(excludeTarget);
                    //if (!result)
                    //{
                    //    Debug.Log("No LOS to " + excludeTarget.name + " because " + hit.transform.name);
                    //    Debug.DrawLine(targetPosition, hit.point, Color.red);
                    //}

                    return result;
                }
            }
            //Debug.Log("Has LOS");
            return true;
        }


        //public static bool HasLineOfSight(this Vector3 source, Vector3 targetPosition, Transform target, int layerMask = -1)
        //{
        //    var hit = new RaycastHit2D();

        //    var rayDirection = targetPosition - source;

        //    //Debug.DrawRay(source, rayDirection, Color.green);

        //    if (Physics2D.Raycast(source, rayDirection, out hit, rayDirection.magnitude, layerMask))
        //    {
        //        var result = hit.transform == target || hit.transform.IsChildOf(target);
        //        //if (!result)
        //        //{
        //        //    //Debug.Log("No LOS to " + target.name + " because " + hit.transform.name);
        //        //    //Debug.DrawLine(targetPosition, hit.point, Color.red);
        //        //}

        //        return result;
        //    }
        //    return true;
        //}

        public static bool HasLineOfSight(this Vector3 source, Transform target, int layerMask = -1, float maxDistance = 100)
        {
            var rayDirection = target.position - source;
            var mag = rayDirection.magnitude;
            if (mag > maxDistance)
                return false;

            //Debug.DrawRay(source, rayDirection, Color.green);
            //var hits = Physics2D.RaycastAll(source, rayDirection, mag, layerMask);
            //foreach (var hit in hits)
            //{

            //    var result = hit.transform == target || hit.transform.IsChildOf(target);
            //    if (!result)
            //    {
            //        Debug.Log("No LOS to " + target.name + " because " + hit.transform.name);
            //        Debug.DrawLine(target.position, hit.point, Color.red);
            //        return false;
            //    }

            //    //return result;
            //}

            var lastHitTriggers = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = false;
            var hit = Physics2D.Raycast(source, rayDirection, mag, layerMask);
            Physics2D.queriesHitTriggers = lastHitTriggers;

            if (hit.collider != null)
            {
                var result = hit.transform == target || hit.transform.IsChildOf(target);
                //if (!result)
                //{
                //    Debug.Log("No LOS to " + target.name + " because " + hit.transform.name);
                //    Debug.DrawLine(target.position, hit.point, Color.red);
                //}

                return result;
            }
            return true;
        }

        //public static Damageable FindClosestTarget(this Vector3 source, bool isPlayer, int layerMask = -1)
        //{
        //    Damageable result = null;
        //    var resultDistanceSquared = float.MaxValue;

        //    //var targets = isPlayer ? Player.Instance.Targets : Level.Instance.CurrentRoom.LiveEnemies;
        //    //foreach (var target in targets)
        //    //{
        //    var target = Player.Instance.Health;
        //        if (target != null)
        //        {
        //            var distanceSquared = (source - target.transform.position).sqrMagnitude;
        //            if (distanceSquared < resultDistanceSquared && !target.IsDead && source.HasLineOfSight(target.transform, layerMask))
        //            {
        //                result = target;
        //                resultDistanceSquared = distanceSquared;
        //            }
        //        }
        //    //}
        //    //print("Found target");
        //    return result;
        //}

        public static bool IsHigherThan(this Transform source, float amount)
        {
            return !Physics.Raycast(source.position, Vector3.down, amount);
        }

        public static void IgnoreCollisionsWith(this GameObject source, GameObject target, bool ignoring = true)
        {
            if (target != null)
            {
                foreach (var targetCollider in target.GetComponentsInChildren<Collider2D>())
                {
                    foreach (var collider in source.GetComponentsInChildren<Collider2D>())
                    {
                        if (collider.enabled)
                        {
                            //Debug.Log(collider + " ignoring " + targetCollider);
                            Physics2D.IgnoreCollision(collider, targetCollider, ignoring);
                        }
                    }
                }
            }
        }

        public static void UpdateGage(GridLayoutGroup gage, Sprite[] sprites, float value, float max)
        {
            if (gage != null)
            {
                var count = 0;
                foreach (Transform sprite in gage.transform)
                {
                    count++;
                    var image = sprite.GetComponent<Image>();
                    if (count > max)
                    {
                        image.enabled = false;
                    }
                    else
                    {
                        image.enabled = true;

                        if (value >= count)
                        {
                            image.sprite = sprites[2];
                        }
                        else if (value > count - 1)
                        {
                            image.sprite = sprites[1];
                        }
                        else
                        {
                            image.sprite = sprites[0];
                        }
                    }
                }
            }
        }

        static bool isExceptionHandlingSetup;
        public static void SetupExceptionHandling()
        {
            if (!isExceptionHandlingSetup)
            {
                isExceptionHandlingSetup = true;
                Application.logMessageReceived += HandleException;
            }

            //throw new Exception();
        }

        public static void HandleException(Exception exception)
        {
            Debug.LogException(exception);
            HandleException("Error", exception.ToString(), LogType.Exception);
        }

        public static Queue<string> ConsoleLog = new Queue<string>();

        public static void AddConsoleLog(string message)
        {
            ConsoleLog.Enqueue(message);
            if (ConsoleLog.Count > 30)
                ConsoleLog.Dequeue();
        }

        public static void HandleException(string condition, string stackTrace, LogType type)
        {
            AddConsoleLog(type.ToString() + ": " + condition);

            //if (condition.StartsWith("Failed to send internal buffer")) // happens on a disconnect
            //    return;

            //if (condition.StartsWith("Send Error: WrongConnection")) // happens on a disconnect
            //    return;

            //if (condition.StartsWith("NATTraversal"))
            //    return;

            if (condition.StartsWith("ServerDisconnected")) // because UNET OnServerDisconnect is stupid and requires base to be called
                return;
            if (condition.StartsWith("ClientDisconnected")) // because UNET OnClientDisconnect is stupid and requires base to be called
                return;

            switch (type)
            {
                case LogType.Exception:
#if !UNITY_EDITOR
                    WebServiceClient.Report(condition, stackTrace, type);
#else
                    //WebServiceClient.Report(condition, stackTrace, type);
#endif
                    //HUD.Instance.ShowMessage(condition, Utility.Red);
                    break;
                case LogType.Assert:
                case LogType.Error:
                case LogType.Warning:
                    //HUD.Instance.ShowMessage(condition, Utility.Lime);
                    break;
                    //case LogType.Log:
                    //    HUD.Instance.ShowMessage(condition);
                    //    break;
            }
        }

        public static void GiveAchievement(string id)
        {
#if UNITY_STANDALONE
            //if (SteamManager.Initialized)
            //{
            //    var achieved = false;
            //    Steamworks.SteamUserStats.GetAchievement(id, out achieved);
            //    if (!achieved)
            //    {
            //        //Debug.Log("Giving Achievement " + id);
            //        /*var setAchievementResult = */
            //        Steamworks.SteamUserStats.SetAchievement(id);
            //        /*var storeStatsResult = */
            //        Steamworks.SteamUserStats.StoreStats();
            //        //Debug.Log("Set Achievement: " + setAchievementResult + " Store Stats: " + storeStatsResult);
            //    }
            //}
#endif
        }

        public static Texture2D CreateScreenshot(Camera camera, int width, int height)
        {
            if (camera == null)
                return null;

            var rt = new RenderTexture(width, height, 24);
            camera.targetTexture = rt;
            var screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            GameObject.Destroy(rt);
            screenShot.Apply(); // so that it can be shown on screen
            return screenShot;
        }

        public static Texture2D CreateScreenshotAlpha(Camera camera, int width, int height)
        {
            var rt = new RenderTexture(width, height, 32);
            camera.targetTexture = rt;
            var screenShot = new Texture2D(width, height, TextureFormat.RGBA32, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            GameObject.Destroy(rt);
            screenShot.Apply(); // so that it can be shown on screen
            return screenShot;
        }

        public static void SetLayer(this Transform trans, int layer)
        {
            trans.gameObject.layer = layer;
            foreach (Transform child in trans)
                child.SetLayer(layer);
        }

        public static Damageable FindClosestTarget(this Vector3 source, TargetType targetType, int layerMask = -1, float maxDistance = 20f)
        {
            Damageable result = null;
            var resultDistanceSquared = maxDistance * maxDistance;
            var targets = Damageable.Damageables.Where((d) => d.IsType(targetType) && !d.IsDead);
            foreach (var target in targets)
            {
                //Debug.Log("checking " + target);
                var distanceSquared = (source - target.transform.position).sqrMagnitude;
                if (distanceSquared < resultDistanceSquared && !target.IsDead && source.HasLineOfSight(target.transform, layerMask))
                {
                    result = target;
                    resultDistanceSquared = distanceSquared;
                }
            }
            //Debug.Log("Found target");
            return result;
        }

        //public static Player GetPlayer(int photonUserId)
        //{
        //    return (from p in Player.Players where p.photonView.owner.ID == photonUserId select p).FirstOrDefault();
        //}

        public static GameObject AddCallout(this Transform parent, string richText)
        {
            var result = GameObject.Instantiate(Level.Instance.CalloutPrefab, parent.position, parent.rotation);
            result.transform.SetParent(parent);

            var text = result.GetComponentInChildren<Text>();
            text.text = richText;

            var typer = result.GetComponentInChildren<TypewriterEffect>();
            if (typer != null)
                typer.Restart();

            return result;
        }

        public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius)
        {
            var dir = (body.transform.position - explosionPosition);
            float wearoff = 1 - (dir.magnitude / explosionRadius);
            body.AddForce(dir.normalized * explosionForce * wearoff);
        }

        public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, float upliftModifier)
        {
            var dir = (body.transform.position - explosionPosition);
            float wearoff = 1 - (dir.magnitude / explosionRadius);
            Vector3 baseForce = dir.normalized * explosionForce * wearoff;
            body.AddForce(baseForce);

            float upliftWearoff = 1 - upliftModifier / explosionRadius;
            Vector3 upliftForce = Vector2.up * explosionForce * upliftWearoff;
            body.AddForce(upliftForce);
        }

        public static string _(this string value)
        {
            return value;
            //var originalValue = value;
            //if (value.Contains("\n"))
            //    value = value.Replace("\n", "\\n");

            //var result = I2.Loc.LocalizationManager.GetTranslation(value);
            //if (result == null)
            //{
            //    if (!String.IsNullOrEmpty(value))
            //        Debug.Log("Translation not found for \"" + value + "\"");
            //    return originalValue;
            //}
            //else
            //{
            //    result = result.Replace("\\n", "\n");
            //    return result;
            //}
        }
    }
}