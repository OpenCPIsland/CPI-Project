Configuration
=============

This folder contains initialization configuration settings for systems. This configuration is independent of GCS, and is more used for bootstrapping the game.

The following configuration files are listed by the highest priority. Each configuration file may contain a override subset of the lower priority configuration settings.

1) DeviceConfig.txt
2) LocalConfig.txt
3) ApplicationConfig.txt

For example if ApplicationConfig.txt contained a setting that disabled logging, LocalConfig.txt could override that setting to enable it, simply by adding the same mapping in the LocalConfig.txt file for the logging system.