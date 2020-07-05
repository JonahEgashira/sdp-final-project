using Sequence = System.Collections.IEnumerator;


public sealed class Game : GameBase
{

  const int screen_width = 768;
  const int screen_height = 1024;

  ////////////// PLAYER INFO /////////////

  float player_x = screen_width / 2;
  float player_y = screen_height / 2;
  float player_speed = 20.0f;

  int player_radius = 20; // depends on the image

  ////////////// BOX INFO //////////////

  const int box_num = 10;

  int[] box_x = new int[box_num];
  int[] box_y = new int[box_num];
  int[] box_speed = new int[box_num];


  ////////// TYPE //////////
  // 0 : normal box (1 damage when hit)
  // 1 : heal box (heal 1 hp when hit)
  // 2 : shield box(take no damage for seconds)
  // 3 : speed up
  // 4 : bomb (destroy all boxes on screen)
  
  int[] box_type = new int[box_num];
  bool[] box_alive_flag = new bool[box_num];

  int box_width = 24;
  int box_height = 24;


  //////////// SCORE AND STAGE(LEVEL) //////////////
  int score = 0;
  int time = 0;
  int stage = 0;


  //////////// GAME STATE AND TITLE SCREEN /////////
  int hp = 0;
  bool is_game_start = true;
  bool is_game_over = false;


  public override void InitGame()
  {
    gc.SetResolution(screen_width, screen_height);

    // INITIALIZE BLOCKS 
    for (int i = 0; i < box_num; i++) {
      box_x[i] = gc.Random(0, screen_width - player_radius);
      box_y[i] = -gc.Random(100, 480);
      box_speed[i] = gc.Random(3, 8);
      box_alive_flag[i] = true;

      // GENERATE BOX TYPE 
      int type = gc.Random(0, 100);
      if(type <= 90) box_type[i] = 0;
      else if(type <= 93) box_type[i] = 1;
      else if(type <= 96) box_type[i] = 2;
      else if(type <= 98) box_type[i] = 3;
      else if(type <= 100) box_type[i] = 4; 
    }

  }

  public override void UpdateGame()
  {

    if(is_game_over == true) {
      InitGame();
      return;
    }

    //////// PLAYER MOVEMENT WITH ACCELERATION /////////

    player_x += gc.AccelerationLastX * player_speed;
    player_y -= gc.AccelerationLastY * player_speed;

    //////// PLAYER HIT EDGES OF SCREEN /////////////////
    if (player_x < 0) player_x = 0;
    if (player_x > 720 - 20) player_x = 720 - 20;
    if (player_y < 0) player_y = 0;
    if (player_y > 1280 - 20) player_y = 1280 - 20;

    time++;
    score = time / 60;

    for (int i = 0; i < box_num; i++) {
      if(gc.CheckHitRect((int)player_x, (int)player_y, 32, 32, box_x[i], box_y[i], box_width, box_height)) {
        if(box_type[i] == 0) {
          hp--;
        }
        if(box_type[i] == 1) {
          hp++;
        }
        if(box_type[i] == 2) {
          // SHIELD 
          // 取ってから一定時間の処理が必要
        }
        if(box_type[i] == 3) {
          // SPEED UP
          // 取ってから一定時間の処理が必要
        }
        if(box_type[i] == 5) {
          // BOMB
        }
      }
    }

  }


  public override void DrawGame()
  {
    gc.ClearScreen();
    gc.SetColor(0,0,0);
    gc.SetFontSize(36);

    if(is_game_start == false && is_game_over == false) {
      gc.DrawRightString("TAP TO PLAY", screen_width / 2, screen_height / 2);
    } else if(is_game_start == true) {
      for (int i = 0; i < box_num; i++) {

        if(box_type[i] == 0) gc.SetColor(0,0,0);
        if(box_type[i] == 1) gc.SetColor(0,255,128);
        if(box_type[i] == 2) gc.SetColor(0,0,0);
        if(box_type[i] == 3) gc.SetColor(0,0,0);
        if(box_type[i] == 4) gc.SetColor(0,0,0);

        gc.FillRect(box_x[i], box_y[i], box_width, box_height);
      }
      gc.SetColor(0,0,0);
      gc.DrawString("SCORE: " + score, 0, 36);
      gc.DrawString("HP: " + hp, 0, 64);
    } else if(is_game_over == true) {
      gc.DrawString("GAME OVER", screen_width / 2, screen_height / 2);
      gc.DrawString("SCORE: " + score, 0, 36);
      gc.DrawString("HP:" + hp, 0, 64);      
    }

  }

}
