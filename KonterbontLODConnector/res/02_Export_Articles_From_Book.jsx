#target "indesign"
#targetengine "com.twixlmedia.publisher.idserver"
#include "zz_twixlForServer.jsx";

var magazineFolderPath = "K:\\Magazines\\";

var myBookContents = TMUtilities.collectionToArray(app.activeBook.bookContents);
var myBookContentsCount = myBookContents.length;
for (var ii = 0; ii < myBookContentsCount; ii++) {
    var myFile = myBookContents[ii];
    var exportFileNamePath = myFile.fullName.name.split('.')[0];
    $.writeln(exportFileNamePath);
    if (ii == 0) {
        //Cover, gets the "Folder"
        magazineFolderPath += myFile.name.substr(0, 7) + "\\export\\articles\\";
    }
    if (myFile.fullName.name.indexOf('Impressum') == -1) {
        var docRef = app.open(myFile.fullName);
        var tmpFileName = myFile.fullName.fsName.replace('/', '//'); //.replace ("/k/", with)
        var argss = new Array(myFile.fullName.fsName, magazineFolderPath + exportFileNamePath + ".article");
        TwixlPublisherPluginAPI.setExportFormat("pdf");
        TwixlPublisherPluginAPI.exportArticleInternal(argss);

        docRef.close();
    }
}
