using System.Collections;
using Mainframe.Scripts.Microgame;
using UnityEngine;

public class Window : MonoBehaviour
{
   private CloseWindowsMicroGame _closeWindowMicrogame;
   private AudioSource _audio;
    
   private void Start()
   {
      _closeWindowMicrogame = Microgame.GetInstance() as CloseWindowsMicroGame;
      _audio = GetComponent<AudioSource>();
   }
   
   public void OnHit()
   {
      _closeWindowMicrogame.CloseWindow(this);
      StartCoroutine(Dying());
   }

   private IEnumerator Dying()
   {
      _audio.Play();
      var t = 0f;
      while(t < 1)
      {
         transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(transform.localScale.x, 0),t);
         t += Time.deltaTime ;
         yield return null;
      }
      yield return new WaitForSeconds(0.1f);
      gameObject.SetActive(false);
   }
}
