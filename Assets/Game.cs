using Sequence = System.Collections.IEnumerator;


public sealed class Game : GameBase {
  const int screen_width = 1125;
  const int screen_height = 2436;

  ////////////// PLAYER INFO /////////////

  float player_x = screen_width / 2;
  float player_y = screen_height - screen_height / 24;
  float player_speed = 20.0f;

  int player_radius = 20; // depends on the image

  ////////////// BOX INFO //////////////

  const int box_num = 10;

  int[] box_x = new int[box_num];
  int[] box_y = new int[box_num];
  int[] box_speed = new int[box_num];

  bool is_bomb_activated = false;


  ////////// TYPE //////////
  // 0 : normal box (1 damage when hit)
  // 1 : heal box (heal 1 hp when hit)
  // 2 : shield box(take no damage for seconds)
  // 3 : speed up
  // 4 : bomb (destroy all boxes on screen)

  int[] box_type = new int[box_num];
  bool[] box_alive_flag = new bool[box_num];

  int box_width = 48;
  int box_height = 48;


  //////////// SCORE AND STAGE(LEVEL) //////////////
  int score = 0;
  int time = 0;
  int stage = 0;

  //////////// GAME STATE AND TITLE SCREEN /////////
  int hp = 1;
  bool is_game_start = true;
  bool is_game_over = false;

  public override void InitGame() {
    gc.SetResolution(screen_width, screen_height);

    // INITIALIZE BLOCKS 
    for (int i = 0; i < box_num; i++) {
      box_x[i] = gc.Random(0, screen_width - player_radius);
      box_y[i] = -gc.Random(100, 480);
      box_speed[i] = gc.Random(3, 8);
      box_alive_flag[i] = true;

      // GENERATE BOX TYPE 
      int type = gc.Random(0, 100);
      if (type <= 60) box_type[i] = 0;
      else if (type <= 70) box_type[i] = 1;
      else if (type <= 80) box_type[i] = 2;
      else if (type <= 90) box_type[i] = 3;
      else if (type <= 100) box_type[i] = 4;
    }
  }

  public override void UpdateGame() {

    if(is_game_over == true) {
      if (gc.GetPointerFrameCount(0) == 1) {
        InitGame();
        is_game_over = false;
        is_game_start = true;
      } 
    }

    is_bomb_activated = false;

    //////// PLAYER MOVEMENT WITH ACCELERATION /////////

    player_x += gc.AccelerationLastX * player_speed;
    // player_y -= gc.AccelerationLastY * player_speed;

    //////// PLAYER HIT EDGES OF SCREEN /////////////////
    if (player_x < 0) player_x = 0;
    if (player_x > screen_width - 20) player_x = screen_width - 20;

    //if (player_y < 0) player_y = 0;
    //if (player_y > screen_height - 20) player_y = screen_height - 20;

    time++;
    score = time / 60;

    for (int i = 0; i < box_num; i++) {

      box_y[i] = box_y[i] + box_speed[i];

      if (box_y[i] > screen_height) {
        box_x[i] = gc.Random(0, screen_width - player_radius);
        box_y[i] = -gc.Random(100, 480);
        box_speed[i] = gc.Random(3, 8);
        box_alive_flag[i] = true;

        int type = gc.Random(0, 100);
        if (type <= 60) box_type[i] = 0;
        else if (type <= 70) box_type[i] = 1;
        else if (type <= 80) box_type[i] = 2;
        else if (type <= 90) box_type[i] = 3;
        else if (type <= 100) box_type[i] = 4;
      }

      if (gc.CheckHitRect((int)player_x, (int)player_y, 32, 32, box_x[i], box_y[i], box_width, box_height)) {
        if (box_alive_flag[i] == false) continue;

        if (box_type[i] == 0) {
          hp--;
        }
        if (box_type[i] == 1) {
          hp++;
        }
        if (box_type[i] == 2) {
          // SHIELD 
          // 取ってから一定時間の処理が必要
        }
        if (box_type[i] == 3) {
          // SPEED UP
          // 取ってから一定時間の処理が必要
        }
        if (box_type[i] == 4) {
          // BOMB
          is_bomb_activated = true;
        }
        box_alive_flag[i] = false;
      }
    }

    if(is_bomb_activated == true) {
      for (int i = 0; i < box_num; i++) {
        box_y[i] = screen_height;
      }
    }

    if (hp <= 0) {
      is_game_over = true;
    }

  }



  public override void DrawGame() {

    gc.ClearScreen();
    gc.SetColor(0, 0, 0);
    gc.SetFontSize(36);

    if (is_game_start == false && is_game_over == false) {
      gc.DrawRightString("TAP TO PLAY", screen_width / 2, screen_height / 2);
    } else if (is_game_start == true && is_game_over == false) {

      for (int i = 0; i < box_num; i++) {

        if (box_alive_flag[i] == false) continue;

        if (box_type[i] == 0) gc.SetColor(0, 0, 0);
        if (box_type[i] == 1) gc.SetColor(0, 255, 0);
        if (box_type[i] == 2) gc.SetColor(0, 255, 255);
        if (box_type[i] == 3) gc.SetColor(0, 0, 255);
        if (box_type[i] == 4) gc.SetColor(255, 0, 0);

        gc.FillRect(box_x[i], box_y[i], box_width, box_height);
      }

      gc.DrawImage(1, (int)player_x, (int)player_y);

      gc.SetColor(0, 0, 0);
      gc.DrawString("SCORE: " + score, 0, 36);
      gc.DrawString("HP: " + hp, 0, 64);

    } else if (is_game_over == true) {
      gc.DrawString("GAME OVER", screen_width / 2, screen_height / 2);
      gc.DrawString("SCORE: " + score, 0, 36);
      gc.DrawString("HP:" + hp, 0, 64);
    }

  }

}
