#targetengine "session"
#include "getWords.jsx"
#include "json2.jsx"

var theFileName = app.activeDocument.name.substr(0,7)+"_";
var myCharacterStyleName = "Marker";
var myScriptPath = getScriptFolder();

var finishedWords=new Array();

theDocCharStyles = app.activeDocument.characterStyles;

createms();

var _docStories = app.activeDocument.stories;

var myBounds;
var markerColor;
var _countword = new Array();
var _count = 0;
for (var c=0; c < _docStories.length; c++)
{ 
    var _docStorie = _docStories[c];
    var _textRanges = _docStorie.textStyleRanges;

    for (j=0; j < _textRanges.length; j++)
    {
        var _textRange = _textRanges[j];


        var _spread = _textRange.parentTextFrames[0].parentPage.parent.index;
        if (_textRange.appliedCharacterStyle.name == "Marker")
        {
            currentTimeDate = new Date();
            markerColor = _textRange.appliedCharacterStyle.underlineColor.colorValue;
            var _found = -1;
            for (var _checkword = 0; _checkword < _count; _checkword++)
            {
				if (_countword[_checkword][0] == _textRange.contents)
				{
				_found = _checkword;
				}
            }
            if ((_found == -1))// || (_count == 0) )
            {
                _countword[_count] = new Array();
                _countword[_count][0] = _textRange.contents;
                _countword[_count][1] = 0;
                _found = _count;
                _count++;
            }
            else
            {
                _countword[_found][1]++;
            }
			app.select(_textRange);
			mySelection = app.selection[0].createOutlines(false);
			myBounds = mySelection[0].geometricBounds;
			mySelection[0].remove();


             var popupLayer = app.activeDocument.layers.itemByName("Popup");
             app.activeDocument.activeLayer=popupLayer;

			switch (_countword[_found][1])
			{
			   case 0: {  setPopup("V", _spread, _found); createbutton(myBounds,_spread,_textRange.contents,"V"); break;}
			   case 1: {  setPopup("H", _spread, _found); createbutton(myBounds,_spread,_textRange.contents,"H"); break;}
			   case 2: {  setPopup("P", _spread, _found); createbutton(myBounds,_spread,_textRange.contents,"P"); break;}
			}
        }
    }
}

savewords();

function setPopup(mod, _thespread, _found)
{
       // CallTooltip(_textRange.contents, null, mod, null, theOS, _cRuns, _found);
}

function createbutton(GB,_theSpread,_theButtonName, _pageSel)
{
   var popupLayer = app.activeDocument.layers.itemByName("Buttons");
   app.activeDocument.activeLayer=popupLayer;
   var _button = app.activeDocument.spreads[_theSpread].buttons.add();
       GB[0] = GB[0] - 5;
       GB[1] = GB[1] - 10;
       GB[2] = GB[2] + 15;
       GB[3] = GB[3] + 10;
       _button.geometricBounds = GB;
       _button.name = _theButtonName+_pageSel;
   if (_pageSel =="P")
   {
        // TWIXL BUTTON
        var number = Math.floor(Math.random()*123456+1);
        var myProperties = {
                'woUrl':                  'webresource://popupbase-web-resources/'+theFileName+'popup_'+deUmlaut(_theButtonName)+'.html',
                'woAllowUserInteraction': true,
                'woWidth':                '260',
                'woHeight':               '420',
                'woShowScrollbars':       false,
                'woBackgroundColor':      '000000',
                'woBackgroundOpacity':    50,
                'woAnalyticsName':        ''+number+'',
                'woShowLoadingIndicator' : true
            }
        var myJsonString = JSON.stringify(myProperties);
         _button.insertLabel("com.rovingbird.epublisher.wo", myJsonString);
         // Wierder sammelen
         finishedWords.push(_theButtonName);
    } else {
      var number = Math.floor(Math.random()*123456+1);
      var myProperties = {
              'woUrl':                  'webresource://popupbase-web-resources/'+theFileName+'popup_'+deUmlaut(_theButtonName)+'H.html',
              'woAllowUserInteraction': true,
              'woWidth':                '500',
              'woHeight':               '154',
              'woShowScrollbars':       false,
              'woBackgroundColor':      '000000',
              'woBackgroundOpacity':    50,
              'woAnalyticsName':        ''+number+'',
              'woShowLoadingIndicator' : true
          }
      var myJsonString = JSON.stringify(myProperties);
       _button.insertLabel("com.rovingbird.epublisher.wo", myJsonString);
    }
}

function getScriptFolder(){
     try {
          var activeScript = File(app.activeScript);
     } catch(e) {
          // running from ESTK ...
          var activeScript = File(e.fileName);
     }
     return activeScript.parent.fsName.toString();
}


function createms()
{
       /*while (app.activeDocument.multiStateObjects.length > 0)
    {
        app.activeDocument.multiStateObjects[0].remove();
    }*/
     for (var i = 0; i < app.activeDocument.spreads.length-1; i++)
    {
            var buttonLayer = app.activeDocument.layers.itemByName("Buttons");
            app.activeDocument.activeLayer=buttonLayer;
            buttonLayer.buttons.everyItem().remove(); 
        }
}

function deUmlaut(value){
  value = value.toLowerCase();
  value = value.replace(/ä/gi, 'ae');
  value = value.replace(/ö/gi, 'oe');
  value = value.replace(/ü/gi, 'ue');
  value = value.replace(/ß/gi, 'ss');
  value = value.replace(/é/gi, 'e');
  value = value.replace(/è/gi, 'e');
  value = value.replace(/ä/gi, 'a');
  value = value.replace(/ü/gi, 'u');
  value = value.replace(/ö/gi, 'o');
  value = value.replace(/ë/gi, 'e');
  value = value.replace(/ê/gi, 'e');
  value = value.replace(/â/gi, 'a');
  value = value.replace(/ /gi, '_');
  value = value.replace(/'/gi, '');
  
  return value;
}

function savewords() {
  finishedWords.sort();
  myFile =  File(app.activeDocument.filePath + "/"+theFileName+".txt");
  myFile.encoding = "UTF-8";
  myFile.open("w");    
  myFile.writeln("_wierder_	");
  myFile.writeln(markerColor);
  for (var i=0;i<finishedWords.length;i++)
    {
        if (finishedWords[i].length>0)
        {
            myFile.writeln(finishedWords[i]);
        }
    }
  //myFile.write("\r");
  myFile.close();
}