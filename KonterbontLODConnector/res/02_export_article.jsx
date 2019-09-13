//#target "indesign"
//#targetengine "com.twixlmedia.publisher.idserver"
#targetengine "session"
#include "zz_twixlForServer.jsx";

//app.userInteractionLevel = UserInteractionLevel.DONTDISPLAYALERTS;

var magazineFolderPath = arguments[0] + arguments[1] + "\\export\\articles\\";

var myFile = app.activeDocument;
var exportFileNamePath = myFile.fullName.name.split('.')[0];
//$.writeln(exportFileNamePath);

var tmpFileName = myFile.fullName.fsName.replace('/', '//'); //.replace ("/k/", with)
var argss = new Array(myFile.fullName.fsName, magazineFolderPath + exportFileNamePath + ".article");
TwixlPublisherPluginAPI.setExportFormat("pdf");
TwixlPublisherPluginAPI.exportArticleInternal(argss);