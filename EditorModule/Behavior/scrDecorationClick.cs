using System;
using ADOFAI;
using UnityEngine;
using UnityModManagerNet;

namespace RandomTweaksEditorModule.Behavior
{
    public class scrDecorationClick : MonoBehaviour
    {
        public LevelEvent Event;
        public scrFloor Floor;
        public float holding = 0;
        public static LevelEvent SelectedEvent;
        private void Awake()
        {
            Physics.queriesHitTriggers = true;
        }

        void Update()
        {
            if (Input.GetMouseButton(1) && RandomTweaksEditorModule.settings.EnableDecorationClickMove)
            {
                Time.timeScale = 1;
                holding += Time.deltaTime;
                if (holding >= 0.2f)
                {
                    double CameraPosX = Math.Round(Camera.current.transform.position.x / 1.5f, 2);
                    double CameraPosY = Math.Round(Camera.current.transform.position.y / 1.5f, 2);
                    double DecoPosX = Math.Round(transform.position.x / 1.5f, 2);
                    double DecoPosY = Math.Round(transform.position.y / 1.5f, 2);
                    float CameraSize = Camera.current.orthographicSize;
                    double MousePosX =
                        Math.Round(
                            (Input.mousePosition.x - Screen.width / 2.0f) / 162 * 1920 / Screen.width * CameraSize / 5,
                            2) + CameraPosX;
                    double MousePosY =
                        Math.Round(
                            (Input.mousePosition.y - Screen.height / 2.0f) / 162 * 1080 / Screen.height * CameraSize /
                            5, 2) + CameraPosY;
                    if (SelectedEvent == null)
                    {

                        Vector2 SelectedPos = Vector2.zero;
                        float positionX = (float) Math.Abs(DecoPosX - MousePosX) * 1.5f;
                        float positionY = (float) Math.Abs(DecoPosY - MousePosY) * 1.5f;
                        //  L.og(new Vector2(positionX, positionY));
                        float sizeX = transform.localScale.x / 2;
                        float sizeY = transform.localScale.y / 2;
                        var component = gameObject.GetComponent<scrDecoration>();
                        if (component is scrDecoration)
                        {
                            sizeX *= ((scrVisualDecoration) component).spriteRenderer.sprite.texture.width;
                            sizeY *= ((scrVisualDecoration) component).spriteRenderer.sprite.texture.height;
                            sizeX /= 100;
                            sizeY /= 100;
                        }

                        if (positionX < sizeX && Mathf.Abs(positionY) < sizeY)
                        {
                            SelectedEvent = Event;
                        }
                        else SelectedEvent = null;
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                        {
                            var position = new Vector2((float) (MousePosX + CameraPosX) * 1f,
                                (float) (MousePosY + CameraPosY) * 1f);
                            SelectedEvent.data["position"] = position;
                            SelectedEvent.data["relativeTo"] = DecPlacementType.Global;
                        }
                        else
                        {
                            var floor = Floor;
                            var position = new Vector2(
                                (float) Math.Round(MousePosX + CameraPosX - floor.transform.position.x * 0.5, 2) * 1f,
                                (float) Math.Round(MousePosY + CameraPosY - floor.transform.position.y * 0.5, 2) * 1f);
                            SelectedEvent.data["position"] = position;
                            SelectedEvent.data["relativeTo"] = DecPlacementType.Tile;
                        }

                    }

                    CustomLevel.instance.UpdateDecorationSprites();
                }
            }

            if (Input.GetMouseButtonUp(1) && RandomTweaksEditorModule.settings.EnableDecorationClickToEvent)
            {
                SelectedEvent = null;
                if (holding > 0.2f) { 
                    holding = 0f;
                }
                else
                {
                    UnityModManager.Logger.Log("Why");
                    double CameraPosX = Math.Round(Camera.current.transform.position.x / 1.5f, 2);
                    double CameraPosY = Math.Round(Camera.current.transform.position.y / 1.5f, 2);
                    double DecoPosX = Math.Round(transform.position.x / 1.5f, 2);
                    double DecoPosY = Math.Round(transform.position.y / 1.5f, 2);
                    float CameraSize = Camera.current.orthographicSize;
                    double MousePosX =
                        Math.Round(
                            (Input.mousePosition.x - Screen.width / 2.0f) / 162 * 1920 / Screen.width * CameraSize / 5,
                            2) + CameraPosX;
                    double MousePosY =
                        Math.Round(
                            (Input.mousePosition.y - Screen.height / 2.0f) / 162 * 1080 / Screen.height * CameraSize /
                            5, 2) + CameraPosY;

                    Vector2 SelectedPos = Vector2.zero;
                    float positionX = (float) Math.Abs(DecoPosX - MousePosX) * 1.5f;
                    float positionY = (float) Math.Abs(DecoPosY - MousePosY) * 1.5f;
                    UnityModManager.Logger.Log(new Vector2(positionX, positionY).ToString());
                    float sizeX = transform.localScale.x / 2;
                    float sizeY = transform.localScale.y / 2;
                    var component = gameObject.GetComponent<scrDecoration>();
                    UnityModManager.Logger.Log("Why2");
                    if (component is scrDecoration)
                    {
                        sizeX *= ((scrVisualDecoration) component).spriteRenderer.sprite.texture.width;
                        sizeY *= ((scrVisualDecoration) component).spriteRenderer.sprite.texture.height;
                        sizeX /= 100;
                        sizeY /= 100;
                    }

                    if (positionX < sizeX && Mathf.Abs(positionY) < sizeY)
                    {
                        scnEditor.instance.selectedFloor = Floor;
                        Camera.current.transform.LocalMoveXY(scnEditor.instance.selectedFloor.transform.position.x,
                            scnEditor.instance.selectedFloor.transform.position.y);
                    }
                }
            }
        }   
    }
}