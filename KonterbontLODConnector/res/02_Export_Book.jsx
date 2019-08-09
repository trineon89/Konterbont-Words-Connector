#target "indesign"
#targetengine "com.twixlmedia.publisher.idserver"
#include "zz_twixlForServer.jsx";

var magazineFolderPath = "K:\\Magazines\\";
var destinationPath = magazineFolderPath + app.activeBook.name.substr(0, 7) + "\\export\\" + app.activeBook.name.substr(0, 7) + ".publication";
var myFile = app.activeBook.fullName.fsName;

TwixlPublisherPluginAPI.exportPublication(app.activeBook, destinationPath);

