// Decompiled with JetBrains decompiler
// Type: Jet_HUD
// Assembly: Jet_HUD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BC6F5E9B-72BD-46B6-962A-D8636B568238
// Assembly location: C:\Users\fowle\Downloads\Jet_HUD.dll

using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

public class Jet_HUD : Script
{
  private const string ScriptName = "JET HUD";
  private const string ScriptVer = "1.3b";
  private const string ScriptAuthor = "Kryo4lex";
  private string f_Aircrafts;
  private ScriptSettings Config;
  private bool modEnabled;
  private bool reallifeDateTime;
  private bool displayArtificialHorizon;
  private bool display3DRadar;
  private bool displayMinimapRadar;
  private bool displayMissileWarnSystem;
  private bool staticArtificialHorizon;
  private float xOffsetRight;
  private int glblTextureDrawIndex;
  private int correctedWidth;
  private string f_Settings;
  private List<string> SupportedAircraftList;
  private bool blipResetNecessary;
  private Vehicle Aircraft;
  private Ped PlayerPed;
  private long aircraftRadarLastTime;
  private long aircraftRadarInterval;
  private bool Timer1Switch;
  private long Timer1SkipTime;
  private long Timer1SkipLast;
  private float heightBef;
  private const int alpha = 255;
  private Color color_HUD;
  private float gpsX;
  private float gpsY;
  private long gpsRefreshRate;
  private long gpsLast;
  private long lastTimeGetAllVehicles;
  private long getAllVehiclesInterval;
  private Vehicle[] vehicles;
  private int rpgRocketHash;
  private int homingRocketHash;
  private int hunterGuidedRocketHash;
  private int hunterBarrageRocketHash;
  private long lastTimeGetAllProps;
  private long getAllPropsInterval;
  private Prop[] props;
  private List<HudComponent> hudComponentsToRemove;
  private float blipRadius;
  private BlipSprite airplaneSprite;
  private BlipSprite helicopterSprite;
  private BlipColor BlipColor;
  private bool onlyShowAircraftWithDriver;
  private List<Vehicle> vehsWithBlips;
  private string vehicleWeaponBefore;
  private string missileType;
  private int toggle_step;
  private string weaponSelectionText;
  private Dictionary<uint, string> VehicleWeapons;


  public Jet_HUD()
  {
    List<HudComponent> hudComponentList = new List<HudComponent>();
    hudComponentList.Add((HudComponent) 7);
    hudComponentList.Add((HudComponent) 3);
    hudComponentList.Add((HudComponent) 13);
    hudComponentList.Add((HudComponent) 9);
    hudComponentList.Add((HudComponent) 3);
    hudComponentList.Add((HudComponent) 2);
    hudComponentList.Add((HudComponent) 6);
    this.hudComponentsToRemove = hudComponentList;
    this.vehsWithBlips = new List<Vehicle>();
    this.vehicleWeaponBefore = string.Empty;
    this.missileType = "MISL GD";
    this.weaponSelectionText = "MISL GD [GUN]";
    this.VehicleWeapons = new Dictionary<uint, string>()
    {
      {
        2971687502U,
        "VEHICLE_WEAPON_ROTORS"
      },
      {
        1945616459U,
        "VEHICLE_WEAPON_TANK"
      },
      {
        4171469727U,
        "VEHICLE_WEAPON_SPACE_ROCKET"
      },
      {
        3473446624U,
        "VEHICLE_WEAPON_PLANE_ROCKET"
      },
      {
        3800181289U,
        "VEHICLE_WEAPON_PLAYER_LAZER"
      },
      {
        4026335563U,
        "VEHICLE_WEAPON_PLAYER_LASER"
      },
      {
        1259576109U,
        "VEHICLE_WEAPON_PLAYER_BULLET"
      },
      {
        1186503822U,
        "VEHICLE_WEAPON_PLAYER_BUZZARD"
      },
      {
        2669318622U,
        "VEHICLE_WEAPON_PLAYER_HUNTER"
      },
      {
        1566990507U,
        "VEHICLE_WEAPON_ENEMY_LASER"
      },
      {
        3450622333U,
        "VEHICLE_WEAPON_SEARCHLIGHT"
      },
      {
        3530961278U,
        "VEHICLE_WEAPON_RADAR"
      },
      {
        1741783703U,
        "VEHICLE_WEAPON_WATER_CANNON"
      },
      {
        1155224728U,
        "VEHICLE_WEAPON_TURRET_INSURGENT"
      },
      {
        2144528907U,
        "VEHICLE_WEAPON_TURRET_TECHNICAL"
      },
      {
        1097917585U,
        "VEHICLE_WEAPON_NOSE_TURRET_VALKYRIE"
      },
      {
        1638077257U,
        "VEHICLE_WEAPON_PLAYER_SAVAGE"
      },
      {
        0U,
        ""
      }
    };
    //base.\u002Ector();
    this.LoadIniFile();
    this.correctedWidth = (int) (30.0 * (1.77778005599976 / (double) this.GetScreenResolutionRatioX()));
    this.add_Tick(new EventHandler(this.OnTick));
    this.add_KeyDown(new KeyEventHandler(this.OnKeyDown));
    this.add_Aborted(new EventHandler(this.OnAborted));
  }

  private void LoadIniFile()
  {
    try
    {
      this.Config = ScriptSettings.Load(this.f_Settings);
      this.modEnabled = (bool) this.Config.GetValue<bool>("SETTINGS", "modEnabled", true);
      this.reallifeDateTime = (bool) this.Config.GetValue<bool>("SETTINGS", "REALLIFE_DATETIME", false);
      this.displayArtificialHorizon = (bool) this.Config.GetValue<bool>("SETTINGS", "DISPLAY_ARTIFICIAL_HORIZON", true);
      this.display3DRadar = (bool) this.Config.GetValue<bool>("SETTINGS", "DISPLAY_3D_RADAR",  true);
      this.displayMinimapRadar = (bool) this.Config.GetValue<bool>("SETTINGS", "DISPLAY_MINIMAP_RADAR",  true);
      this.displayMissileWarnSystem = (bool) this.Config.GetValue<bool>("SETTINGS", "DISPLAY_MISSILE_WARN_SYSTEM", true);
      this.staticArtificialHorizon = (bool) this.Config.GetValue<bool>("SETTINGS", "STATIC_ARTIFICIAL_HORIZON", false);
      this.gpsRefreshRate = (long) this.Config.GetValue<long>("SETTINGS", "GPS_REFRESH_RATE",  500L);
      this.getAllVehiclesInterval = (long) this.Config.GetValue<long>("SETTINGS", "3D_RADAR_REFRESH_RATE",  10000L);
      this.getAllPropsInterval = (long) this.Config.GetValue<long>("SETTINGS", "MISSILE_WARN_SYSTEM_REFRESH_RATE",  100L);
      this.color_HUD = Color.FromArgb((int) this.Config.GetValue<int>("SETTINGS", "HUD_A",  (int) byte.MaxValue), (int) this.Config.GetValue<int>("SETTINGS", "HUD_R",  67), (int) this.Config.GetValue<int>("SETTINGS", "HUD_G",  197), (int) this.Config.GetValue<int>("SETTINGS", "HUD_B",  0));
      this.xOffsetRight = float.Parse(((string) this.Config.GetValue<string>("SETTINGS", "XOFFSET_RIGHT_ELEMENTS",  "0.0")).Replace(',', '.'), (IFormatProvider) CultureInfo.InvariantCulture);
      this.SupportedAircraftList = new List<string>((IEnumerable<string>) File.ReadAllLines(this.f_Aircrafts));
      this.blipRadius = 10000f;
      this.airplaneSprite = (BlipSprite) 423;
      this.helicopterSprite = (BlipSprite) 422;
      this.BlipColor = (BlipColor) 2;
      this.onlyShowAircraftWithDriver = true;
    }
    catch (Exception ex)
    {
      Notification.Show("~r~Error~w~:Failed to load JET HUD settings.");
    }
  }

  private void OnTick(object sender, EventArgs e)
  {
    this.PlayerPed = Game.get_Player().get_Character();
    this.Aircraft = this.PlayerPed.get_CurrentVehicle();
    if (Entity.op_Inequality((Entity) this.Aircraft, (Entity) null))
    {
      if (this.modEnabled)
      {
        List<string> supportedAircraftList = this.SupportedAircraftList;
        Model model = ((Entity) this.PlayerPed.get_CurrentVehicle()).get_Model();
        string str = ((Model) model).get_Hash().ToString();
        if (supportedAircraftList.Contains(str) && this.PlayerPed.get_CurrentVehicle().get_IsDriveable())
          this.mainFeatures();
      }
    }
    else if (this.blipResetNecessary)
    {
      this.removeBlips();
      this.blipResetNecessary = false;
    }
    this.glblTextureDrawIndex = 0;
  }

  private void OnAborted(object sender, EventArgs eventArgs)
  {
    this.removeBlips();
  }

  private void mainFeatures()
  {
    if ((long) Environment.TickCount >= this.Timer1SkipLast + this.Timer1SkipTime)
    {
      this.Timer1Switch = !this.Timer1Switch;
      this.Timer1SkipLast = (long) Environment.TickCount;
    }
    this.hideUnwantedHUDElements();
    this.drawDateTime();
    this.drawGeometryInfo();
    this.drawMechanicalInfo();
    this.drawGPSInfo();
    this.drawCompass();
    if (this.display3DRadar)
      this.draw3DRadar();
    if (this.displayArtificialHorizon)
      this.drawArtificialHorizon();
    if (this.displayMinimapRadar && this.aircraftRadarLastTime + this.aircraftRadarInterval <= (long) Environment.TickCount)
    {
      this.processAircraftRadar();
      this.aircraftRadarLastTime = (long) Environment.TickCount;
    }
    if (this.displayMissileWarnSystem)
      this.drawMissileWarnSystem();
    this.glblTextureDrawIndex = 1;
  }

  private void OnKeyDown(object sender, KeyEventArgs e)
  {
    switch (e.KeyCode)
    {
      case Keys.D:
        if (this.toggle_step == 2)
        {
          this.modEnabled = !this.modEnabled;
          this.toggle_step = 0;
          this.removeBlips();
          Function.Call(Hash.DISPLAY_HUD, new InputArgument[1]
          {
            InputArgument.op_Implicit(true)
          });
          Function.Call(Hash.DISPLAY_RADAR, new InputArgument[1]
          {
            InputArgument.op_Implicit(true)
          });
          break;
        }
        this.toggle_step = 0;
        break;
      case Keys.H:
        if (this.toggle_step == 0)
        {
          ++this.toggle_step;
          break;
        }
        this.toggle_step = 0;
        break;
      case Keys.U:
        if (this.toggle_step == 1)
        {
          ++this.toggle_step;
          break;
        }
        this.toggle_step = 0;
        break;
      default:
        this.toggle_step = 0;
        break;
    }
    if (!Game.IsControlPressed((GTA.Control) 26) || !Game.IsControlPressed((GTA.Control) 22) || (!Game.IsControlPressed((GTA.Control) 87) || !this.PlayerPed.IsInVehicle()))
      return;
    List<string> supportedAircraftList1 = this.SupportedAircraftList;
    Model model1 = ((Entity) this.PlayerPed.get_CurrentVehicle()).get_Model();
    string str1 = ((Model) model1).get_Hash().ToString();
    if (!supportedAircraftList1.Contains(str1))
    {
      List<string> supportedAircraftList2 = this.SupportedAircraftList;
      Model model2 = ((Entity) this.PlayerPed.get_CurrentVehicle()).get_Model();
      string str2 = ((Model) model2).get_Hash().ToString();
      supportedAircraftList2.Add(str2);
      if (File.Exists(this.f_Aircrafts))
        File.WriteAllLines(this.f_Aircrafts, (IEnumerable<string>) this.SupportedAircraftList);
      Notification.Show("~b~Added vehicle to supported aircrafts for Jet HUD mod.");
    }
    else
      Notification.Show("~b~This vehicle is already part of supported aircrafts for Jet HUD mod.");
  }

  private void drawDateTime()
  {
    float xpos = (float) (1.0 - Function.Call<float>(Hash._GET_ASPECT_RATIO, new InputArgument[1]
    {
      InputArgument.op_Implicit(false)
    }) / 10.0) + this.xOffsetRight;
    float ypos = 0.0f;
    if (this.reallifeDateTime)
    {
      this.drawString2("DATE " + DateTime.Now.ToString("dd MM yy ddd", (IFormatProvider) CultureInfo.InvariantCulture).ToUpper() + "\nTIME " + DateTime.Now.ToString("HH mm ss", (IFormatProvider) CultureInfo.InvariantCulture), xpos, ypos, 0.5f, this.color_HUD, false);
    }
    else
    {
      string str1 = ((int) Function.Call<int>((Hash) 4400224173958044981L, new InputArgument[0])).ToString();
      if (str1.Length == 1)
        str1 = "0" + str1;
      string str2 = (Function.Call<int>((Hash) 0xBBC72712E80257A1, new InputArgument[0]) + 1).ToString();
      if (str2.Length == 1)
        str2 = "0" + str2;
      string str3 = ((int) Function.Call<int>((Hash) 0x961777E64BDAF717, new InputArgument[0])).ToString();
      this.drawString2("DATE " + str1 + " " + str2 + " " + str3 + "\nTIME " + World.get_CurrentDayTime().ToString().Replace(":", " "), xpos, ypos, 0.5f, this.color_HUD, false);
    }
  }

  private void drawGeometryInfo()
  {
    float xpos = 0.0f;
    float ypos = 0.5f;
    this.drawString2("SPD " + (this.Aircraft.get_Speed() * 3.6f).ToString("0"), xpos, ypos, 0.75f, this.color_HUD, false);
    this.drawString2("VSPD " + (((float) ((Entity) this.Aircraft).get_Position().Z - this.heightBef) / Game.get_LastFrameTime()).ToString("0.00", (IFormatProvider) CultureInfo.InvariantCulture), xpos, ypos + 0.04f, 0.75f, this.color_HUD, false);
    this.drawString2("ROL  " + ((float) ((Vector3) Function.Call<Vector3>((Hash) 0xAFBD61CC738D9EB9, new InputArgument[2]
    {
      InputArgument.op_Implicit(this.Aircraft),
      InputArgument.op_Implicit(2)
    })).Y).ToString("0") + "°", xpos, ypos + 0.08f, 0.75f, this.color_HUD, false);
    this.drawString2("PIT   " + ((float) ((Entity) this.Aircraft).get_Rotation().X).ToString("0") + "°", xpos, ypos + 0.12f, 0.75f, this.color_HUD, false);
    this.heightBef = (float) ((Entity) this.Aircraft).get_Position().Z;
  }

  private void drawMechanicalInfo()
  {
    this.getSelectedAircraftWeapons();
    float xpos = (float) (0.975000023841858 - Function.Call<float>((Hash) 0xF1307EF624A80D87, new InputArgument[1]
    {
      InputArgument.op_Implicit(false)
    }) / 10.0) + this.xOffsetRight;
    float ypos = 0.88f;
    if (Jet_HUD.GetVehicleCurrentWeapon(this.PlayerPed) != 0U)
      this.drawString2(this.weaponSelectionText, xpos, ypos - 0.04f, 0.75f, this.color_HUD, false);
    this.drawString2("HGHT " + ((Entity) this.Aircraft).get_Position().Z.ToString("0"), xpos, ypos, 0.75f, this.color_HUD, false);
    Color colorHud1 = this.color_HUD;
    string str1 = "GEAR DOWN";
    if (this.Aircraft.get_LandingGear() == 1 || this.Aircraft.get_LandingGear() == 2)
    {
      if (this.Aircraft.get_LandingGear() == 1)
        str1 = "GEAR CLSNG";
      if (this.Aircraft.get_LandingGear() == 2)
        str1 = "GEAR OPNG";
      Color color = Color.FromArgb(204, 204, 0);
      if (this.Timer1Switch)
        this.drawString2(str1, xpos, ypos + 0.04f, 0.75f, color, false);
    }
    else if (this.Aircraft.get_LandingGear() == null)
    {
      string str2 = "GEAR DOWN";
      Color colorHud2 = this.color_HUD;
      this.drawString2(str2, xpos, ypos + 0.04f, 0.75f, colorHud2, false);
    }
    else if (this.Aircraft.get_LandingGear() == 3)
    {
      string str2 = "GEAR UP";
      Color colorHud2 = this.color_HUD;
      this.drawString2(str2, xpos, ypos + 0.04f, 0.75f, colorHud2, false);
    }
    if (this.Aircraft.get_EngineRunning() || this.Aircraft.get_IsStopped())
    {
      this.drawString2("ENG  " + (this.Aircraft.get_EngineRunning() ? "ON" : "OFF"), xpos, ypos + 0.08f, 0.75f, this.color_HUD, false);
    }
    else
    {
      if (!this.Timer1Switch)
        return;
      this.drawString2("ENG  " + (this.Aircraft.get_EngineRunning() ? "ON" : "OFF"), xpos, ypos + 0.08f, 0.75f, Color.FromArgb(204, 204, 0), false);
    }
  }

  private void drawGPSInfo()
  {
    if ((long) Environment.TickCount >= this.gpsLast + this.gpsRefreshRate)
    {
      this.gpsX = (float) ((Entity) this.Aircraft).get_Position().X;
      this.gpsY = (float) ((Entity) this.Aircraft).get_Position().Y;
      this.gpsLast = (long) Environment.TickCount;
    }
    this.drawString2("GPS DTA\nLNG " + this.gpsX.ToString("0") + "\nLAT " + this.gpsY.ToString("0") + "\n" + World.GetZoneName(((Entity) this.Aircraft).get_Position()).ToUpper(), 0.0f, 0.0f, 0.5f, this.color_HUD, false);
  }

  private float getAngleBetween2Points2D(Vector3 p1, Vector3 p2)
  {
    float num = (float) (System.Math.Atan2((double) System.Math.Abs(System.Math.Abs((float) p1.Y) - System.Math.Abs((float) p2.Y)), (double) System.Math.Abs(System.Math.Abs((float) p1.X) - System.Math.Abs((float) p2.X))) * (180.0 / System.Math.PI));
    Vector3 vector3 = Vector3.Subtract(p1, p2);
    if (!this.isPositive((float) vector3.X) || !this.isPositive((float) vector3.Y))
    {
      if (!this.isPositive((float) vector3.X) && this.isPositive((float) vector3.Y))
        num = 180f - num;
      else if (!this.isPositive((float) vector3.X) && !this.isPositive((float) vector3.Y))
        num = 180f + num;
      else if (this.isPositive((float) vector3.X) && !this.isPositive((float) vector3.Y))
        num = 360f - num;
    }
    return num;
  }

  private void drawCompass()
  {
    float y = 0.1f;
    float num1 = (float) ((Entity) this.Aircraft).get_Rotation().Z;
    if ((double) num1 < 0.0)
      num1 = 360f + num1;
    float num2 = 360f - num1;
    float num3 = 0.0f;
    char ch = 'L';
    bool isWaypointActive = Game.get_IsWaypointActive();
    if (isWaypointActive)
    {
      float num4 = this.getAngleBetween2Points2D(((Entity) this.Aircraft).get_Position(), World.GetWaypointPosition()) - this.getAngleBetween2Points2D(Vector3.op_Addition(((Entity) this.Aircraft).get_Position(), Vector3.op_Multiply(((Entity) this.Aircraft).get_ForwardVector(), 10f)), ((Entity) this.Aircraft).get_Position());
      float num5 = num4;
      if ((double) num4 < -180.0)
      {
        num5 = num4 + 180f;
        num3 = num2 - num5;
        ch = 'R';
      }
      else if ((double) num4 < 0.0)
      {
        num5 = num4 + 180f;
        num3 = num2 + -num5;
        ch = 'L';
      }
      else if ((double) num4 >= 180.0)
      {
        num5 = num4 - 180f;
        num3 = num2 + -num5;
        ch = 'L';
      }
      else if ((double) num4 >= 0.0)
      {
        num5 = num4 - 180f;
        num3 = num2 - num5;
        ch = 'R';
      }
      if ((double) num3 < 0.0)
        num3 = 360f + num3;
      if ((double) num3 > 360.0)
        num3 -= 360f;
      switch (ch)
      {
        case 'L':
          this.drawString2((double) num5 < 90.0 ? ((double) num5 < 50.0 ? "" : "<") : "<<", 0.275f, y - 0.025f, 0.65f, this.color_HUD, true);
          break;
        case 'R':
          this.drawString2((double) System.Math.Abs(num5) < 90.0 ? ((double) System.Math.Abs(num5) < 50.0 ? "" : ">") : ">>", 0.725f, y - 0.025f, 0.65f, this.color_HUD, true);
          break;
      }
    }
    string str1 = (double) num2 < 0.0 || (double) num2 >= 22.5 ? ((double) num2 < 22.5 || (double) num2 >= 67.5 ? ((double) num2 < 67.5 || (double) num2 >= 112.5 ? ((double) num2 < 112.5 || (double) num2 >= 157.5 ? ((double) num2 < 157.5 || (double) num2 >= 202.5 ? ((double) num2 < 202.5 || (double) num2 >= 247.5 ? ((double) num2 < 247.5 || (double) num2 >= 292.5 ? ((double) num2 < 292.5 || (double) num2 >= 337.5 ? "N" : "NW") : "W") : "SW") : "S") : "SE") : "E") : "NE") : "N";
    this.drawString2("HDG " + num2.ToString("0") + "° " + str1, 0.5f, 0.0f, 0.5f, this.color_HUD, true);
    this.drawRect(0.5f, y, 0.4f, 3f / 1000f, this.color_HUD);
    this.drawRect(0.5f, y + 0.015f, 3f / 1000f, 0.03f, this.color_HUD);
    float num6 = (float) ((Entity) this.Aircraft).get_Rotation().Z;
    if ((double) num6 < 0.0)
      num6 = 360f + num6;
    float num7 = (float) (((double) ((int) num6 % 10) + ((double) num6 - System.Math.Truncate((double) num6))) / 10.0);
    int num8 = (int) num2 / 10;
    int num9 = num8 - 5;
    if (num9 < 0)
      num9 = 36 + num9;
    int num10 = num8 + 5;
    if (num10 > 35)
      num10 = System.Math.Abs(36 - num10);
    bool flag = false;
    for (int index = 0; index <= 9; ++index)
    {
      int num4 = num10 - num9 != 10 ? (num9 + index >= 36 ? System.Math.Abs(36 - (num9 + index + 1)) : num9 + index + 1) : num9 + index + 1;
      if (num4 == 36)
        num4 = 0;
      string str2 = num4.ToString();
      if (num4 == 0)
        str2 = "N";
      if (num4 == 9)
        str2 = "E";
      if (num4 == 18)
        str2 = "S";
      if (num4 == 27)
        str2 = "W";
      if (num4 % 3 == 0)
      {
        this.drawString2(str2, (float) (0.300000011920929 + (double) index * 0.0399999991059303 + (double) num7 * 0.0399999991059303), y - 0.06f, 0.5f, this.color_HUD, true);
        this.drawRect((float) (0.300000011920929 + (double) index * 0.0399999991059303 + (double) num7 * 0.0399999991059303), y - 0.015f, 3f / 1000f, 0.03f, this.color_HUD);
      }
      else
        this.drawRect((float) (0.300000011920929 + (double) index * 0.0399999991059303 + (double) num7 * 0.0399999991059303), y - 0.0075f, 3f / 1000f, 0.015f, this.color_HUD);
      if (isWaypointActive)
      {
        int num5 = (int) num3 / 10;
        float num11 = (float) (((double) ((int) num3 % 10) + ((double) num3 - System.Math.Truncate((double) num3))) / 10.0);
        if (num4 == num5 && !flag)
        {
          if (num5 != num10)
          {
            this.drawString2("V", (float) (0.300000011920929 + (double) index * 0.0399999991059303 + (double) num7 * 0.0399999991059303 + (double) num11 * 0.0399999991059303), y - 0.04f, 0.8f, this.color_HUD, true);
            this.drawString2("NAV", (float) (0.300000011920929 + (double) index * 0.0399999991059303 + (double) num7 * 0.0399999991059303 + (double) num11 * 0.0399999991059303), y + 0.03f, 0.5f, this.color_HUD, true);
          }
          else if (1.0 - (double) num7 > (double) num11)
          {
            this.drawString2("V", (float) (0.300000011920929 + (double) index * 0.0399999991059303 + (double) num7 * 0.0399999991059303 + (double) num11 * 0.0399999991059303), y - 0.04f, 0.8f, this.color_HUD, true);
            this.drawString2("NAV", (float) (0.300000011920929 + (double) index * 0.0399999991059303 + (double) num7 * 0.0399999991059303 + (double) num11 * 0.0399999991059303), y + 0.03f, 0.5f, this.color_HUD, true);
          }
          flag = true;
        }
        if (num5 == num9 && !flag && 1.0 - (double) num7 < (double) num11)
        {
          this.drawString2("V", (float) (0.260000020265579 + (double) num7 * 0.0399999991059303 + (double) num11 * 0.0399999991059303), y - 0.04f, 0.8f, this.color_HUD, true);
          this.drawString2("NAV", (float) (0.260000020265579 + (double) num7 * 0.0399999991059303 + (double) num11 * 0.0399999991059303), y + 0.03f, 0.5f, this.color_HUD, true);
          flag = true;
        }
      }
    }
  }

  private void draw3DRadar()
  {
    if ((long) Environment.TickCount >= this.lastTimeGetAllVehicles + this.getAllVehiclesInterval)
    {
      if (this.vehicles != null)
        Array.Clear((Array) this.vehicles, 0, this.vehicles.Length);
      this.vehicles = Array.FindAll<Vehicle>(World.GetAllVehicles(), (Predicate<Vehicle>) (v =>
      {
        Model model1 = ((Entity) v).get_Model();
        if (!((Model) model1).get_IsPlane())
        {
          Model model2 = ((Entity) v).get_Model();
          if (!((Model) model2).get_IsHelicopter())
          {
            Model model3 = ((Entity) v).get_Model();
            if (!((Model) model3).get_IsCargobob())
              goto label_5;
          }
        }
        if (Entity.op_Inequality((Entity) v, (Entity) this.Aircraft))
        {
          Vector3 position = ((Entity) v).get_Position();
          return (double) position.DistanceTo(((Entity) this.Aircraft).get_Position()) <= (double) this.blipRadius;
        }
label_5:
        return false;
      }));
      this.lastTimeGetAllVehicles = (long) Environment.TickCount;
    }
    int num1 = 1;
    foreach (Vehicle vehicle in this.vehicles)
    {
      if (Entity.op_Inequality((Entity) vehicle, (Entity) this.Aircraft) && vehicle.get_IsDriveable() && ((Entity) vehicle).get_IsOnScreen())
      {
        if (Function.Call<bool>((Hash) 0xFCDFF7B72D23A1AC, new InputArgument[3]
        {
          InputArgument.op_Implicit(vehicle),
          InputArgument.op_Implicit(this.Aircraft),
          InputArgument.op_Implicit(17)
        }) != null && !((Entity) vehicle).get_IsOccluded())
        {
          Point screen = UI.WorldToScreen(((Entity) vehicle).get_Position());
          float xpos = (float) screen.X / (float) UI.WIDTH;
          float num2 = (float) screen.Y / (float) UI.HEIGHT;
          if (screen.X != 0 && screen.Y != 0)
          {
            UI.DrawTexture("scripts//Jet_HUD//Rectangle.png", num1, 1, 100, UI.WorldToScreen(((Entity) vehicle).get_Position()), new PointF(0.5f, 0.5f), new Size(this.correctedWidth, 30), 0.0f, this.color_HUD);
            ++num1;
            Vector3 position = ((Entity) vehicle).get_Position();
            this.drawString2("\nDST: " + position.DistanceTo(((Entity) this.Aircraft).get_Position()).ToString("0"), xpos, num2 - 0.085f, 0.5f, this.color_HUD, true);
          }
        }
      }
    }
  }

  private void drawArtificialHorizon()
  {
    float y = (float) ((Vector3) Function.Call<Vector3>((Hash) 0xAFBD61CC738D9EB9, new InputArgument[2]
    {
      InputArgument.op_Implicit(this.Aircraft),
      InputArgument.op_Implicit(2)
    })).Y;
    Point screen = UI.WorldToScreen(Vector3.op_Addition(((Entity) this.Aircraft).get_Position(), Vector3.op_Multiply(((Entity) this.Aircraft).get_ForwardVector(), 100f)));
    float num1 = (float) -((double) screen.Y / (double) (float) UI.HEIGHT - 0.5);
    float num2 = (float) ((double) screen.X / (double) (float) UI.WIDTH - 0.5);
    if (this.staticArtificialHorizon)
    {
      num1 = 0.0f;
      num2 = 0.0f;
    }
    ((Entity) this.Aircraft).get_Rotation();
    bool flag = true;
    if (screen.X == 0 && screen.X == 0 || Game.IsControlPressed((GTA.Control) 26))
      flag = false;
    float num3 = 0.3f;
    float num4 = 0.1f;
    float standardisedPitch = this.getStandardisedPitch(this.Aircraft);
    float num5 = (float) (((double) ((int) standardisedPitch % 10) + ((double) standardisedPitch - System.Math.Truncate((double) standardisedPitch))) / 10.0);
    int num6 = (int) standardisedPitch - (int) standardisedPitch % 10;
    int num7 = 4;
    if (!flag)
      return;
    for (int index1 = 0; index1 <= num7; ++index1)
    {
      int num8 = 0;
      switch (index1)
      {
        case 0:
          num8 = num6 + 20;
          break;
        case 1:
          num8 = num6 + 10;
          break;
        case 2:
          num8 = num6;
          break;
        case 3:
          num8 = num6 - 10;
          break;
        case 4:
          num8 = num6 - 20;
          break;
      }
      if (num8 > 90)
        num8 = 90 - (num8 - 90);
      else if (num8 < -90)
        num8 = -(num8 + 90) - 90;
      string str = num8.ToString("0");
      if (str == "0")
      {
        this.drawRect(0.3825f + num2, (float) ((double) num3 + (double) index1 * (double) num4 - (double) num1 + (double) num5 * (double) num4), 0.11f, 1f / 500f, this.color_HUD);
        this.drawRect(0.6175f + num2, (float) ((double) num3 + (double) index1 * (double) num4 - (double) num1 + (double) num5 * (double) num4), 0.11f, 1f / 500f, this.color_HUD);
      }
      else
      {
        if (str[0] == '-')
        {
          for (int index2 = 0; index2 <= 5; ++index2)
          {
            this.drawRect((float) (0.400000005960464 + (double) (index2 * 2) * (1.0 / 160.0) + (double) num2 - 0.0343750007450581), (float) ((double) num3 + (double) index1 * (double) num4 - (double) num1 + (double) num5 * (double) num4), 1f / 160f, 1f / 500f, this.color_HUD);
            this.drawRect((float) (0.600000023841858 + (double) (index2 * 2) * (1.0 / 160.0) + (double) num2 - 0.0281249992549419), (float) ((double) num3 + (double) index1 * (double) num4 - (double) num1 + (double) num5 * (double) num4), 1f / 160f, 1f / 500f, this.color_HUD);
          }
        }
        else
        {
          this.drawRect(0.4f + num2, (float) ((double) num3 + (double) index1 * (double) num4 - (double) num1 + (double) num5 * (double) num4), 0.075f, 1f / 500f, this.color_HUD);
          this.drawRect(0.6f + num2, (float) ((double) num3 + (double) index1 * (double) num4 - (double) num1 + (double) num5 * (double) num4), 0.075f, 1f / 500f, this.color_HUD);
        }
        this.drawRect(0.3625f + num2, (float) ((double) num3 + (double) index1 * (double) num4 - (double) num1 + (double) num5 * (double) num4), 1f / 500f, 0.009735f, this.color_HUD);
        this.drawRect(0.6375f + num2, (float) ((double) num3 + (double) index1 * (double) num4 - (double) num1 + (double) num5 * (double) num4), 1f / 500f, 0.009735f, this.color_HUD);
      }
      this.drawString2(str, 0.3625f + num2, (float) ((double) num3 - 0.0350000001490116 + (double) index1 * (double) num4 - (double) num1 + (double) num5 * (double) num4), 0.4f, this.color_HUD, true);
      this.drawString2(str, 0.6375f + num2, (float) ((double) num3 - 0.0350000001490116 + (double) index1 * (double) num4 - (double) num1 + (double) num5 * (double) num4), 0.4f, this.color_HUD, true);
    }
    UI.DrawTexture("scripts//Jet_HUD//W.png", this.glblTextureDrawIndex, 1, 100, new Point(640 + (int) ((double) num2 * 1280.0), 360 - (int) ((double) num1 * 720.0)), new PointF(0.5f, 0.5f), new Size(104, 60), y / 360f, this.color_HUD, this.GetScreenResolutionRatio());
    ++this.glblTextureDrawIndex;
  }

  private void drawMissileWarnSystem()
  {
    if ((long) Environment.TickCount >= this.lastTimeGetAllProps + this.getAllPropsInterval)
    {
      if (this.props != null)
        Array.Clear((Array) this.props, 0, this.props.Length);
      this.props = World.GetAllProps();
      this.lastTimeGetAllProps = (long) Environment.TickCount;
    }
    foreach (Entity prop in this.props)
    {
      Model model = prop.get_Model();
      if (((Model) model).get_Hash() != this.rpgRocketHash)
      {
        model = prop.get_Model();
        if (((Model) model).get_Hash() != this.homingRocketHash)
        {
          model = prop.get_Model();
          if (((Model) model).get_Hash() != this.hunterGuidedRocketHash)
          {
            model = prop.get_Model();
            if (((Model) model).get_Hash() != this.hunterBarrageRocketHash)
              continue;
          }
        }
      }
      if (prop.get_IsOnScreen())
      {
        if (Function.Call<bool>((Hash) 0xFCDFF7B72D23A1AC, new InputArgument[3]
        {
          InputArgument.op_Implicit(prop),
          InputArgument.op_Implicit(this.Aircraft),
          InputArgument.op_Implicit(17)
        }) != null && !prop.get_IsOccluded())
        {
          Vector3 vector3 = prop.get_Velocity();
          if ((double) ((Vector3) ref vector3).Length() > 1.0)
          {
            Point screen = UI.WorldToScreen(prop.get_Position());
            float xpos = (float) screen.X / (float) UI.WIDTH;
            float num1 = (float) screen.Y / (float) UI.HEIGHT;
            if (screen.X != 0 && screen.Y != 0)
            {
              vector3 = prop.get_Position();
              float num2 = ((Vector3) ref vector3).DistanceTo(((Entity) this.Aircraft).get_Position());
              vector3 = prop.get_Position();
              this.drawString2("\nMISL DST: " + ((Vector3) ref vector3).DistanceTo(((Entity) this.Aircraft).get_Position()).ToString("0"), xpos, num1 - 0.085f, 0.5f, Color.Red, true);
              float scale = (float) (150.0 / (double) num2 * 1.5);
              if ((double) num2 <= 150.0)
                scale = 1.5f;
              this.drawString(';'.ToString(), xpos, num1 - 0.04f, scale, Color.Red, true);
            }
          }
        }
      }
    }
  }

  private void hideUnwantedHUDElements()
  {
    using (List<HudComponent>.Enumerator enumerator = this.hudComponentsToRemove.GetEnumerator())
    {
      while (enumerator.MoveNext())
        UI.HideHudComponentThisFrame(enumerator.Current);
    }
  }

  private void processBlips(Ped plrPed)
  {
    if (this.vehsWithBlips.Count != 0)
    {
      for (int index = 0; index < this.vehsWithBlips.Count; ++index)
      {
        Vehicle vehsWithBlip = this.vehsWithBlips[index];
        if (!Entity.op_Equality((Entity) vehsWithBlip, (Entity) null) && ((Entity) vehsWithBlip).Exists() && ((Entity) vehsWithBlip).get_IsAlive() && (!this.onlyShowAircraftWithDriver || !vehsWithBlip.IsSeatFree((VehicleSeat) -1)))
        {
          Vector3 position = ((Entity) vehsWithBlip).get_Position();
          if ((double) ((Vector3) ref position).DistanceTo(((Entity) plrPed).get_Position()) <= (double) this.blipRadius)
          {
            ((Entity) this.vehsWithBlips[index]).get_CurrentBlip().set_Rotation((int) ((Entity) vehsWithBlip).get_Heading());
            continue;
          }
        }
        if (Blip.op_Inequality(((Entity) this.vehsWithBlips[index]).get_CurrentBlip(), (Blip) null) && ((Entity) this.vehsWithBlips[index]).get_CurrentBlip().Exists())
          ((Entity) this.vehsWithBlips[index]).get_CurrentBlip().Remove();
        this.vehsWithBlips.RemoveAt(index);
      }
    }
    this.blipResetNecessary = true;
  }

  private void processAircraftRadar()
  {
    Ped playerPed = this.PlayerPed;
    this.processBlips(playerPed);
    Vehicle[] nearbyVehicles = World.GetNearbyVehicles(playerPed, this.blipRadius);
    if (nearbyVehicles != null && nearbyVehicles.Length != 0)
    {
      foreach (Vehicle vehicle in nearbyVehicles)
      {
        if (Entity.op_Inequality((Entity) vehicle, (Entity) null) && !this.vehsWithBlips.Contains(vehicle) && (((Entity) vehicle).Exists() && ((Entity) vehicle).get_IsAlive()) && (!((Entity) vehicle).get_CurrentBlip().Exists() && (!this.onlyShowAircraftWithDriver || !vehicle.IsSeatFree((VehicleSeat) -1))))
        {
          Model model = ((Entity) vehicle).get_Model();
          bool isPlane;
          if (((isPlane = ((Model) model).get_IsPlane()) || ((Model) model).get_IsHelicopter()) && Model.op_Inequality(model, Model.op_Implicit("polmav")))
          {
            Blip blip = ((Entity) vehicle).AddBlip();
            if (vehicle.get_DisplayName().ToUpper() == "LAZER" || vehicle.get_DisplayName().ToUpper() == "BESRA")
              blip.set_Sprite((BlipSprite) 16);
            else
              blip.set_Sprite(isPlane ? this.airplaneSprite : this.helicopterSprite);
            blip.set_Color(this.BlipColor);
            blip.set_Rotation((int) ((Entity) vehicle).get_Heading());
            this.vehsWithBlips.Add(vehicle);
          }
        }
      }
    }
    else
      this.processBlips((Ped) null);
  }

  private void removeBlips()
  {
    using (List<Vehicle>.Enumerator enumerator = this.vehsWithBlips.GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        Vehicle current = enumerator.Current;
        if (((Entity) current).Exists() && Entity.op_Inequality((Entity) current, (Entity) null) && (Blip.op_Inequality(((Entity) current).get_CurrentBlip(), (Blip) null) && ((Entity) current).get_CurrentBlip().Exists()))
          ((Entity) current).get_CurrentBlip().Remove();
      }
    }
    this.vehsWithBlips.Clear();
  }

  private void drawTexture(
    string filename,
    int index,
    int level,
    int time,
    Point pos,
    PointF center,
    Size size,
    float rotation,
    Color color,
    float aspectRaio)
  {
    UI.DrawTexture(filename, this.glblTextureDrawIndex, level, time, pos, center, size, rotation, color, aspectRaio);
    ++this.glblTextureDrawIndex;
  }

  private void drawString2(
    string str,
    float xpos,
    float ypos,
    float scale,
    Color color,
    bool centered)
  {
    UIText uiText = new UIText(str, new Point((int) ((double) (float) UI.WIDTH * (double) xpos), (int) ((double) (float) UI.HEIGHT * (double) ypos)), scale, color, (Font) 4, centered);
    uiText.set_Enabled(true);
    uiText.Draw();
  }

  private void drawString(
    string str,
    float xpos,
    float ypos,
    float scale,
    Color color,
    bool centered)
  {
    UIText uiText = new UIText(str, new Point((int) ((double) (float) UI.WIDTH * (double) xpos), (int) ((double) (float) UI.HEIGHT * (double) ypos)), scale, color, (Font) 2, centered);
    uiText.set_Enabled(true);
    uiText.Draw();
  }

  public void draw2DBox(
    float x,
    float y,
    float width,
    float height,
    float thickness,
    Color color,
    bool upper,
    bool lower,
    bool left,
    bool right)
  {
    if (upper)
      this.drawRect(x, y, width, thickness, color);
    if (lower)
      this.drawRect(x, y + height, width, thickness, color);
    if (left)
      this.drawRect(x - width / 2f, y + height / 2f, 1f / 500f, height, color);
    if (!right)
      return;
    this.drawRect(x + width / 2f, y + height / 2f, 1f / 500f, height, color);
  }

  public void drawLine(Vector3 from, Vector3 to, Color col)
  {
    Function.Call((Hash) 7742345298724472448L, new InputArgument[10]
    {
      InputArgument.op_Implicit((float) from.X),
      InputArgument.op_Implicit((float) from.Y),
      InputArgument.op_Implicit((float) from.Z),
      InputArgument.op_Implicit((float) to.X),
      InputArgument.op_Implicit((float) to.Y),
      InputArgument.op_Implicit((float) to.Z),
      InputArgument.op_Implicit(col.R),
      InputArgument.op_Implicit(col.G),
      InputArgument.op_Implicit(col.B),
      InputArgument.op_Implicit(col.A)
    });
  }

  public void drawRect(float x, float y, float width, float height, Color color)
  {
    Function.Call((Hash) 4206795403398567152L, new InputArgument[8]
    {
      InputArgument.op_Implicit(x),
      InputArgument.op_Implicit(y),
      InputArgument.op_Implicit(width),
      InputArgument.op_Implicit(height),
      InputArgument.op_Implicit(color.R),
      InputArgument.op_Implicit(color.G),
      InputArgument.op_Implicit(color.B),
      InputArgument.op_Implicit(color.A)
    });
  }

  private void displayHelpTextThisFrame(string text)
  {
    Function.Call((Hash) 0x8509B634FBE7DA11, new InputArgument[1]
    {
      InputArgument.op_Implicit("STRING")
    });
    Function.Call((Hash) 7789129354908300458L, new InputArgument[1]
    {
      InputArgument.op_Implicit(text)
    });
    Function.Call((Hash) 2562546386151446694L, new InputArgument[4]
    {
      InputArgument.op_Implicit(0),
      InputArgument.op_Implicit(0),
      InputArgument.op_Implicit(1),
      InputArgument.op_Implicit(-1)
    });
  }

  private double DegreeToRadian(double angle)
  {
    return System.Math.PI * angle / 180.0;
  }

  private double RadianToDegree(double rad)
  {
    return rad * (180.0 / System.Math.PI);
  }

  public Vector3 rotateVector3OnAxis(Vector3 vector3torotate, float angle, int axis)
  {
    angle = (float) this.DegreeToRadian((double) angle);
    Vector3 vector3;
    ((Vector3) ref vector3).\u002Ector(0.0f, 0.0f, 0.0f);
    switch (axis)
    {
      case 0:
        vector3.X = vector3torotate.X;
        vector3.Y = (__Null) (vector3torotate.Y * System.Math.Cos((double) angle) - vector3torotate.Z * System.Math.Sin((double) angle));
        vector3.Z = (__Null) (vector3torotate.Y * System.Math.Sin((double) angle) + vector3torotate.Z * System.Math.Cos((double) angle));
        return vector3;
      case 1:
        vector3.X = (__Null) (vector3torotate.Z * System.Math.Sin((double) angle) + vector3torotate.X * System.Math.Cos((double) angle));
        vector3.Y = vector3torotate.Y;
        vector3.Z = (__Null) (vector3torotate.Z * System.Math.Cos((double) angle) - vector3torotate.X * System.Math.Sin((double) angle));
        return vector3;
      case 2:
        vector3.X = (__Null) (vector3torotate.X * System.Math.Cos((double) angle) + vector3torotate.Y * System.Math.Sin((double) angle));
        vector3.Y = (__Null) (-vector3torotate.X * System.Math.Cos((double) angle) + vector3torotate.Y * System.Math.Sin((double) angle));
        vector3.Z = vector3torotate.Z;
        return vector3;
      default:
        return vector3;
    }
  }

  private float DegreesToRadians(float degree)
  {
    return degree * ((float) System.Math.PI / 180f);
  }

  private float DirectionToHeadingInRadians(Vector3 dir)
  {
    dir.Z = (__Null) 0.0;
    dir.Normalize();
    return (float) System.Math.Atan2((double) dir.X, (double) dir.Y);
  }

  private Vector3 RotationToDirection(Vector3 Rotation)
  {
    float radians1 = this.DegreesToRadians((float) Rotation.Z);
    float radians2 = this.DegreesToRadians((float) Rotation.X);
    float num = System.Math.Abs((float) System.Math.Cos((double) radians2));
    return new Vector3((float) -System.Math.Sin((double) radians1) * num, (float) System.Math.Cos((double) radians1) * num, (float) System.Math.Sin((double) radians2));
  }

  private void getSelectedAircraftWeapons()
  {
    string str1;
    string str2 = !this.VehicleWeapons.TryGetValue(Jet_HUD.GetVehicleCurrentWeapon(this.PlayerPed), out str1) ? "[GUN]" : str1;
    if (str2 == this.vehicleWeaponBefore && (str2 == "VEHICLE_WEAPON_SPACE_ROCKET" || str2 == "VEHICLE_WEAPON_PLANE_ROCKET"))
    {
      if (Game.IsControlJustPressed(0, (Control) 99))
        this.missileType = "MISL UG";
      if (Game.IsControlJustPressed(0, (Control) 100))
        this.missileType = "MISL GD";
    }
    else
      this.missileType = "MISL GD";
    switch (str2)
    {
      case "VEHICLE_WEAPON_PLANE_ROCKET":
        this.weaponSelectionText = "[" + this.missileType + "] GUN";
        break;
      case "VEHICLE_WEAPON_PLAYER_BUZZARD":
        this.weaponSelectionText = this.missileType + " [GUN]";
        break;
      case "VEHICLE_WEAPON_PLAYER_HUNTER":
        this.weaponSelectionText = this.missileType + " [GUN]";
        break;
      case "VEHICLE_WEAPON_PLAYER_LASER":
        this.weaponSelectionText = this.missileType + " [GUN]";
        break;
      case "VEHICLE_WEAPON_PLAYER_LAZER":
        this.weaponSelectionText = this.missileType + " [GUN]";
        break;
      case "VEHICLE_WEAPON_PLAYER_SAVAGE":
        this.weaponSelectionText = this.missileType + " [GUN]";
        break;
      case "VEHICLE_WEAPON_SPACE_ROCKET":
        this.weaponSelectionText = "[" + this.missileType + "] GUN";
        break;
    }
    this.vehicleWeaponBefore = str2;
  }

  public static uint GetVehicleCurrentWeapon(Ped Ped)
  {
    OutputArgument outputArgument = new OutputArgument();
    Function.Call((Hash) 1159492374221042396L, new InputArgument[2]
    {
      InputArgument.op_Implicit(Ped),
      (InputArgument) outputArgument
    });
    return (uint) outputArgument.GetResult<int>();
  }

  private string getVector3Pos(Vector3 v)
  {
    // ISSUE: explicit non-virtual call
    return "X: " + v.X.ToString("0.0") + "\nY: " + v.Y.ToString("0.0") + "\nZ: " + __nonvirtual (v.Z.ToString());
  }

  private bool isPositive(float number)
  {
    return (double) number >= 0.0;
  }

  public float getStandardisedPitch(Vehicle aircraft)
  {
    float num = (float) ((Entity) aircraft).get_Rotation().X;
    if ((double) num > 90.0)
      num = (float) (90.0 - ((double) num - 90.0));
    else if ((double) num < -90.0)
      num = (float) (-((double) num + 90.0) - 90.0);
    return num;
  }

  public float GetScreenResolutionRatio()
  {
    int width1 = Game.get_ScreenResolution().Width;
    int height1 = Game.get_ScreenResolution().Height;
    Size screenResolution = Game.get_ScreenResolution();
    double width2 = (double) screenResolution.Width;
    screenResolution = Game.get_ScreenResolution();
    double height2 = (double) screenResolution.Height;
    return (float) (width2 / height2);
  }

  public float GetScreenResolutionRatioX()
  {
    int width = Game.get_ScreenResolution().Width;
    int height = Game.get_ScreenResolution().Height;
    return (float) Function.Call<float>((Hash) 0xF1307EF624A80D87, new InputArgument[1]
    {
      InputArgument.op_Implicit(false)
    });
  }
}
