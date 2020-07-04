using Sequence = System.Collections.IEnumerator;


public sealed class Game : GameBase
{
  const int screen_width = 768;
  const int screen_height = 1024;

  ////////////// PLAYER INFO /////////////

  float player_x = screen_width / 2;
  float player_y = screen_height / 2;
  float player_speed = 20.0f;

  int player_width = 20;
  int 

  
  ////////////// BOX INFO //////////////

  const int box_num = 10;
  int[] box_x = new int [box_num];
  int[] box_y = new int [box_num];
  int[] box_speed = new int[box_num];
  int bow_width = 24;
  int box_height = 24;




  public override void InitGame()
  {
    gc.SetResolution(screen_width, screen_height);

  }


  public override void UpdateGame()
  {

    //////// PLAYER MOVEMENT WITH ACCELERATION /////////

    player_x += gc.AccelerationLastX * player_speed;
    player_y -= gc.AccelerationLastY * player_speed;

    //////// PLAYER HIT EDGES OF SCREEN /////////////////
    if (player_x < 0) player_x = 0;
    if (player_x > 720 - 20) player_x = 720 - 20;
    if (player_y < 0) player_y = 0;
    if (player_y > 1280 - 20) player_y = 1280 - 20;

  }


  public override void DrawGame()
  {
    
  }
}
