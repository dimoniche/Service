/*
Navicat MySQL Data Transfer

Source Server         : testserv
Source Server Version : 50713
Source Host           : localhost:3306
Source Database       : servterminal

Target Server Type    : MYSQL
Target Server Version : 50713
File Encoding         : 65001

Date: 2016-06-26 02:28:29
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for logwork
-- ----------------------------
DROP TABLE IF EXISTS `logwork`;
CREATE TABLE `logwork` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `iddev` int(255) NOT NULL,
  `timework` int(11) NOT NULL,
  `datetime` datetime NOT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=cp866;
SET FOREIGN_KEY_CHECKS=1;
