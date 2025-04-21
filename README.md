# ParaPROG
ParaPROG started in about 2018 as a simple USB programmer for older memory chips with parallel interface.
The repository includes files sufficient to reproduce hardware (schematics in PDF and KiCad, Gerber files), compiled firmware and PC application source code (C#) providing some flexibility about adding more supported devices.
The repository includes different versions of hardware/firmware for different chips (located accordingly in subfolders).

# Supported devices
Version ParaPROG 29/39/49-5V:<br/>
---AMIC--<br/>
A29001<br/>
A29010<br/>
A29002<br/>
A290021<br/>
A29040<br/>
---ATMEL--<br/>
AT49F512<br/>
AT49F001(N)<br/>
AT49F001(N)T<br/>
AT49F002(N)<br/>
AT49F002(N)T<br/>
AT49F/HF010<br/>
AT49F020<br/>
AT49F040<br/>
---PMC--<br/>
PM39F010<br/>
PM39F020<br/>
PM39F040<br/>
---ALLIANCE--<br/>
AS29F040<br/>
---WINBOND--<br/>
W49F002<br/>
---SST--<br/>
SST39SF010<br/>
SST39SF020<br/>
SST39SF040<br/>
 ---MXIC--<br/>
MX28F1000P<br/>
MX28F2000P<br/>
MX29F001T<br/>
MX29F001B<br/>
MX29F002T<br/>
MX29F002B<br/>
MX29F040<br/>
---FUJITSU--<br/>
MBM29F002B<br/>
MBM29F002T<br/>
MBM29F002ST<br/>
MBM29F002SB<br/>
MBM29F040<br/>
---HYNIX--<br/>
HY29F002T<br/>
---AMD--<br/>
AM29F010<br/>
AM29F040<br/>

Version ParaPROG 49/39-mod1:<br/>
---SST---<br/>
SST49LF002A/B<br/>
SST49LF003A/B<br/>
SST49LF004A/B<br/>
SST49LF008A/B<br/>
SST49LF004<br/>
SST49LF004B<br/>
SST49LF004C<br/>
SST49LF008C<br/>
SST49LF020<br/>
SST49LF020A<br/>
SST49LF030A<br/>
SST49LF040<br/>
SST49LF040B<br/>
SST49LF080A<br/>
SST49LF016C<br/>
---STM---<br/>
M50LPW002<br/>
M50LPW012<br/>
M50LPW041<br/>
M50LPW080<br/>
M050FW040<br/>
M050FW080<br/>
M50FLW040A<br/>
M50FLW040B<br/>
---Winbond---<br/>
W49V002FA<br/>
W49V002<br/>
W39V040A<br/>
W39V080A<br/>
W39V040FA<br/>
W39V040FB/B<br/>
W39V040FC/C<br/>
W39V080FA<br/>
---ISSI---<br/>
Pm49FL002<br/>
Pm49FL004<br/>
Pm49FL008<br/>
---Amic---<br/>
A49LF040<br/>
A49LF004<br/>
---Atmel---<br/>
AT49LW040<br/>
AT49LW080<br/>
AT49LH002<br/>
AT49LH004<br/>
AT49LH00B4<br/>
---Intel---<br/>
82802AB<br/>
82802AC<br/>

## Hardware
The repository includes Gerber files of previous version of hardware (tested) and schematics in KiCad, which must be sufficient to reproduce hardware.<br/>
![image](IMG/29-39-49-5V.jpg)
![image](IMG/39-49-mod1.jpg)
KiCad layout is still under "TODO" status.

## Software
The software provides access to all basic operations as reading, erasing, writng and verifying data as well as storing memory dump to file.
All versions of device work as VCP (virtual COM port) so no specific drivers required.
Visual Studio Community Edition can be used to build or modify software.<br/>
![image](IMG/screen-1.jpg)
![image](IMG/screen-2.jpg)

## Drivers & settings
The device works as VCP (Virtual COM Port) in all versions. 
For Version ParaPROG 29/39/49-5V - [VCP driver](https://www.st.com/en/development-tools/stsw-stm32102.html) from ST Microelectronics should be used.
For Version ParaPROG 49/39-mod1 - CH340 (USB-UART bridge) driver.

## Firmware
Firmware file can be found in corresponding sub-folders. The most affordable tools to flash firmware: ST-Link (for STM32 version) and USBAVR (for ATmega16 version).


## License
This project is licensed under multiple terms:
- **Hardware (KiCad design files)**: Licensed under **CERN-OHL-W-2.0**, which allows modification and commercial use with attribution.  
- **Firmware (binary-only)**: Licensed under **Apache 2.0**, but the source code is not provided. Modification or reverse-engineering is prohibited.  
- **Software**: Licensed under **MIT**. If you use this software in any form (modified or unmodified), please include a link back to the original repository.

See the ![LICENSE file](LICENSE) for details. 
