using Sequence = System.Collections.IEnumerator;


public sealed class Game : GameBase {
  const int screen_width = 1125;
  const int screen_height = 2436;

  ////////////// PLAYER INFO /////////////

  float player_x = screen_width / 2;
  float player_y = screen_height - screen_height / 24;
  float player_speed = 20.0f;

  const int DEFAULT_PLAYER_RADIUS = 120;
  int player_radius = DEFAULT_PLAYER_RADIUS; // depends on the image

  ////////////// BOX INFO //////////////

  const int MAX_BOX_NUM = 100;

  int box_num = 10;

  int[] box_x = new int[MAX_BOX_NUM];
  int[] box_y = new int[MAX_BOX_NUM];
  int[] box_speed = new int[MAX_BOX_NUM];

  bool is_bomb_activated = false;


  ////////// TYPE //////////
  // 0 : 普通の箱　当たると１ダメージ
  // 1 : 回復ボックス　当たると１回復
  // 2 : なににしようかな
  // 3 : なににしようかな
  // 4 : 爆弾 画面の箱全て消す

  int[] box_type = new int[MAX_BOX_NUM];
  bool[] box_alive_flag = new bool[MAX_BOX_NUM];

  int box_width = 96;
  int box_height = 96;

  int highest_score = 0;

  int score = 0;
  int time = 0;
  int stage = 0;
  int hp = 1;
  bool is_game_start = false;
  bool is_game_over = false;

  int shield_frame_count = 0;
  int small_frame_count = 0;

  int is_shield_activated = 0;
  int is_small_activated = 0;

  int img_index = 0;

  public override void InitGame() {
    gc.SetResolution(screen_width, screen_height);
    init();
  }


  ////////////////////////////////////////////////////////////

  public override void UpdateGame() {

    //ゲーム開始の最初の画面
    if(is_game_start == false && is_game_over == false) {
      if(gc.GetPointerFrameCount(0) == 1) {
        init();
        is_game_start = true;
      }
      return;
    }

    // ゲームオーバー処理
    if(is_game_over == true) {
      if(gc.GetPointerFrameCount(0) == 1) {
        init();
        is_game_over = false;
        is_game_start = true; 
      }
      return;
    }

    if(is_small_activated == 1) {
      player_radius = DEFAULT_PLAYER_RADIUS / 2;
    } else {
      player_radius = DEFAULT_PLAYER_RADIUS;
    }

    // 爆弾のアイテムを取ったとき
    is_bomb_activated = false;

    if(shield_frame_count > 0) shield_frame_count--;
    if(small_frame_count > 0) small_frame_count--;

    if(shield_frame_count > 0) is_shield_activated = 1;
    else is_shield_activated = 0;

    if(small_frame_count > 0) is_small_activated = 1;
    else is_small_activated = 0;

    if(is_small_activated == 1) {
      
    }

    // プレイヤーの動き
    player_x += gc.AccelerationLastX * player_speed;
    // player_y -= gc.AccelerationLastY * player_speed;

    // x 方向にしか動かないようにする
    if (player_x < 0) player_x = 0;
    if (player_x > screen_width - player_radius) {
      player_x = screen_width - player_radius;
    }


    // time , score , stageの処理
    time++;
    score = time / 60;


    box_num = (score + 10 - 1) / 10 * 10;
    if(box_num > MAX_BOX_NUM) box_num = MAX_BOX_NUM;


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
 
      if (gc.CheckHitRect((int)player_x, (int)player_y, player_radius / 2, player_radius / 2, box_x[i], box_y[i], box_width, box_height)) {
        if (box_alive_flag[i] == false) continue;

        if (box_type[i] == 0 && is_shield_activated == 0) {
          hp--;
        }
        if (box_type[i] == 1) {
          hp++;
        }
        if (box_type[i] == 2 && is_small_activated == 0) {
          // SHIELD 
          // 取ってから一定時間の処理が必要
          shield_frame_count += 180;
        }
        if (box_type[i] == 3 && is_shield_activated == 0) {
          // 小さくなる
          // 取ってから一定時間の処理
          small_frame_count += 180;
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
        box_y[i] = screen_height + 100;
      }
    }

    if(score > highest_score) {
      highest_score = score;
    }

    if (hp <= 0) {
      is_game_over = true;
      is_game_start = false;
      gc.Save(0, highest_score);
    }

  }


  ////////////////////////////////////////////////////////////

  public override void DrawGame() {

    gc.ClearScreen();
    gc.SetColor(0, 0, 0);
    gc.SetFontSize(36);

    if (is_game_start == false && is_game_over == false) {
      gc.DrawString("タップしてスタート", screen_width / 2 - 200, screen_height / 2);
      gc.DrawString("かたむけて、よけろ！", screen_width / 2 - 200, screen_height / 2 + 100);

      gc.SetColor(0, 0, 0);
      gc.FillRect(screen_width / 2 - 200, screen_height / 2 + 200, box_width, box_height);
      gc.DrawString("当たると１ダメージ", screen_width / 2, screen_height / 2 + 200);

      gc.SetColor(0, 255, 0);
      gc.FillRect(screen_width / 2 - 200, screen_height / 2 + 300, box_width, box_height);
      gc.DrawString("当たると１回復", screen_width / 2, screen_height / 2 + 300);

      gc.SetColor(0, 255, 255);
      gc.FillRect(screen_width / 2 - 200, screen_height / 2 + 400, box_width, box_height);
      gc.DrawString("シールドを５秒間得る", screen_width / 2, screen_height / 2 + 400);

      gc.SetColor(0, 0, 255);
      gc.FillRect(screen_width / 2 - 200, screen_height / 2 + 500, box_width, box_height);
      gc.DrawString("５秒間小さくなる", screen_width / 2, screen_height / 2 + 500);
      
      gc.SetColor(255, 0, 0);
      gc.FillRect(screen_width / 2 - 200, screen_height / 2 + 600, box_width, box_height);
      gc.DrawString("画面のすべての箱を消す", screen_width / 2, screen_height / 2 + 600);
      
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

      if(is_shield_activated == 1) {
        img_index = 2;
      }
      else if(is_small_activated == 1) {
        img_index = 1;
      } else {
        img_index = 1;
      }
    
      gc.DrawScaledRotateImage(img_index, (int)player_x, (int)player_y, player_radius, player_radius, 1);

      gc.SetColor(0, 0, 0);
      gc.DrawString("SCORE: " + score, 0, 36);
      gc.DrawString("HP: " + hp, 0, 64);

    } else if (is_game_over == true) {
      gc.DrawString("ゲームオーバー", screen_width / 2 - 200, screen_height / 2);
      gc.DrawString("タップしてリトライ", screen_width / 2 - 200, screen_height / 2 + 100);
      gc.DrawString("SCORE: " + score, screen_width / 2 - 200, screen_height / 2 + 150);
      gc.DrawString("HIGHEST SCORE: " + highest_score, screen_width / 2 - 200, screen_height / 2 + 200);
      gc.DrawString("HP:" + hp, 0, 64);
    }

  }

  void init() {
    // INITIALIZE BLOCKS 
    for (int i = 0; i < MAX_BOX_NUM; i++) {
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

    hp = 1;
    score = 0;
    time = 0;
    stage = 0;

  }

}
//////////////////////////////////////////////////TODO
/*
プレイヤーのいらすとを用意する
１．普通のやつ
２．シールドを付けたバージョン
３．小さくなったバージョン

ライフをハートのイラストとして表示できたらとても良い

*/