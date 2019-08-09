/**
 * ------------------------------------------------------------------------
 * Twixl media Confidential
 * ------------------------------------------------------------------------
 *
 * (c) 2010-2016 Twixl media, http://twixlmedia.com
 *
 * NOTICE: All information contained herein is, and remains the property of
 * Twixl media and its suppliers, if any. The intellectual and technical
 * concepts contained herein are proprietary to Twixl media and its
 * suppliers and may be covered by U.S. and Foreign Patents, patents in
 * process, and are protected by trade secret or copyright law.
 *
 * Dissemination of this information or reproduction of this material is
 * strictly forbidden unless prior written permission is obtained from
 * Twixl media
 */

// Twixl Publisher 9.3 build 32750
#targetengine "com.twixlmedia.publisher.idserver"

/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMAlternateLayouts = {
    create: function (myBook, myRequestedLayouts, myLinkStories, myCopyStyles) {
        try {
            if (myRequestedLayouts.length == 0) {
                return;
            }
            var myProgressSteps = myBook.tmArticleCount();
            TMProgressBar.create('Creating Alternate Layouts', 'Creating Alternate Layouts', myProgressSteps);
            if (myRequestedLayouts === undefined) {
                myRequestedLayouts = this.publicationLayouts(myBook, true);
            }
            TMLogger.info('Creating alternate layouts: ' + myRequestedLayouts.join(', '));
            this.createMissingLayouts(myBook, myRequestedLayouts, myLinkStories, myCopyStyles);
        } catch (myException) {
            TMLogger.exception('TMAlternateLayouts.create', myException + ' (tm_alternatelayout:26)');
            throw myException;
        }
        TMProgressBar.close();
    },
    createForDocument: function (myDocument, myRequestedLayouts, myLinkStories, myCopyStyles) {
        try {
            if (myRequestedLayouts.length == 0) {
                return;
            }
            var myProgressSteps = myRequestedLayouts.length;
            TMProgressBar.create('Creating Alternate Layouts', 'Creating Alternate Layouts', myProgressSteps);
            var myDocumentLayouts = this.documentLayouts(myDocument);
            for (var i = 0; i < myRequestedLayouts.length; i++) {
                var myRequestedLayout = myRequestedLayouts[i];
                if (!TMUtilities.itemInArray(myDocumentLayouts, myRequestedLayout)) {
                    this.createAlternateLayout(myDocument, myDocumentLayouts, myRequestedLayout, myLinkStories, myCopyStyles);
                }
                TMProgressBar.updateProgressBySteps(1);
            };
        } catch (myException) {
            TMLogger.exception('TMAlternateLayouts.create', myException + ' (tm_alternatelayout:53)');
            throw myException;
        }
        TMProgressBar.close();
    },
    createMissingLayouts: function (myBook, myPublicationLayouts, myLinkStories, myCopyStyles) {
        try {
            myBook.tmSaveModifiedDocuments();
            var myOpenFiles = TMFiles.getVisibleFiles();
            var myBookContents = TMUtilities.collectionToArray(app.activeBook.bookContents);
            var myBookContentsCount = myBookContents.length;
            for (var i = 0; i < myBookContentsCount; i++) {
                var myFile = myBookContents[i];
                TMProgressBar.updateLabel("Processing: " + TMUtilities.decodeURI(myFile.name));
                TMProgressBar.updateProgressBySteps(1);
                TMLogger.info("Processing: " + myFile.fullName.fsName);
                if (myDocument = app.open(myFile.fullName, false)) {
                    var myDocumentLayouts = this.documentLayouts(myDocument);
                    for (var j in myPublicationLayouts) {
                        var myPublicationLayout = myPublicationLayouts[j];
                        if (!TMUtilities.itemInArray(myDocumentLayouts, myPublicationLayout)) {
                            this.createAlternateLayout(myDocument, myDocumentLayouts, myPublicationLayout, myLinkStories, myCopyStyles);
                        }
                    }
                    try {
                        if (!TMUtilities.itemInArray(myOpenFiles, myFile.fullName.fsName)) {
                            myDocument.close(SaveOptions.YES);
                        } else {
                            myDocument.save();
                        }
                    } catch (myException) {
                        TMLogger.exception('TMAlternateLayouts.createMissingLayouts', myException + ' (tm_alternatelayout:111)');
                    }
                } else {
                    TMLogger.info('Failed to open document: ' + myFile.fullName.fsName);
                }
            }
        } catch (myException) {
            TMLogger.exception('TMAlternateLayouts.createMissingLayouts', myException + ' (tm_alternatelayout:121)');
            return [];
        }
    },
    createAlternateLayout: function (myDocument, myDocumentLayouts, myLayout, myLinkStories, myCopyStyles) {
        try {
            var width = 1024;
            var height = 768;
            if (myLayout == 'iPad H') {
                width = 1024;
                height = 768;
            } else if (myLayout == 'iPad V') {
                width = 768;
                height = 1024;
            } else if (myLayout == 'Android 10" H') {
                width = 1280;
                height = 752;
            } else if (myLayout == 'Android 10" V') {
                width = 800;
                height = 1232;
            } else if (myLayout == 'Kindle Fire/Nook H') {
                width = 1024;
                height = 552;
            } else if (myLayout == 'Kindle Fire/Nook V') {
                width = 600;
                height = 976;
            } else if (myLayout == 'Phone H') {
                width = 568;
                height = 320;
            } else if (myLayout == 'Phone V') {
                width = 320;
                height = 568;
            } else {
                var myException = new Error('Unsupported alternate layout name: ' + myLayout);
                TMStackTrace.addToStack("TMAlternateLayouts.createAlternateLayout", myException);
            }
            var spreads = this.spreadsForDocument(myDocument, myDocumentLayouts, myLayout);
            if (spreads.length == 1) {
                var firstPage = spreads[0].pages.firstItem();
                var firstPageHeight = firstPage.tmPageHeightPx();
                var screenHeight = firstPage.tmScreenHeightPx();
                if (firstPageHeight > screenHeight) {
                    height = firstPageHeight;
                }
            }
            TMLogger.info('Creating alternate layout: ' + myLayout + ' (' + width + ' x ' + height + ' px)');
            myDocument.createAlternateLayout(
                spreads,
                myLayout,
                width + 'px',
                height + 'px',
                myCopyStyles,
                myLinkStories,
                LayoutRuleOptions.PRESERVE_EXISTING
            );
        } catch (myException) {
            TMLogger.exception('TMAlternateLayouts.createAlternateLayout', myException + ' (tm_alternatelayout:183)');
            throw myException;
        }
    },
    publicationLayouts: function (myBook, myUpdateProgress) {
        try {
            myBook.tmSaveModifiedDocuments();
            var myLayouts = {};
            var myBookContents = TMUtilities.collectionToArray(app.activeBook.bookContents);
            var myBookContentsCount = myBookContents.length;
            for (var i = 0; i < myBookContentsCount; i++) {
                var myFile = myBookContents[i];
                if (myUpdateProgress === true) {
                    TMProgressBar.updateLabel("Checking: " + TMUtilities.decodeURI(myFile.name));
                    TMProgressBar.updateProgressBySteps(1);
                }
                if (!myFile.fullName.exists) {
                    continue;
                }
                if (myDocument = app.open(myFile.fullName, false, OpenOptions.OPEN_COPY)) {
                    var myDocumentLayouts = this.documentLayouts(myDocument);
                    for (var j in myDocumentLayouts) {
                        var myDocumentLayout = myDocumentLayouts[j];
                        if (myLayouts[myDocumentLayout] == undefined) {
                            myLayouts[myDocumentLayout] = 0;
                        }
                        myLayouts[myDocumentLayout]++;
                    }
                    try {
                        myDocument.close(SaveOptions.NO);
                    } catch (myException) {
                        TMLogger.exception('TMAlternateLayouts.publicationLayouts', myException + ' (tm_alternatelayout:225)');
                    }
                }
            }
            return myLayouts;
        } catch (myException) {
            return {};
        }
    },
    documentLayouts: function (myDocument) {
        try {
            var myLayouts = [];
            for (var i = 0; i < myDocument.sections.length; i++) {
                var alternateLayout = myDocument.sections[i].alternateLayout;
                if (alternateLayout != '') {
                    myLayouts.push(alternateLayout);
                }
            };
            return myLayouts;
        } catch (myException) {
            TMLogger.exception('TMAlternateLayouts.documentLayouts', myException + ' (tm_alternatelayout:250)');
            return [];
        }
    },
    spreadsForDocument: function (myDocument, myDocumentLayouts, myName) {
        try {
            var myBaseSection = this.baseForLayout(myDocumentLayouts, myName);
            var mySpreads = [];
            var mySection = myDocument.sections.firstItem();
            if (myBaseSection != undefined) {
                var myCount = myDocument.sections.length;
                for (var i = 0; i < myCount; i++) {
                    var myDocumentSection = myDocument.sections[i];
                    if (myDocument.sections[i].tmLayoutName() == myBaseSection) {
                        mySection = myDocumentSection;
                        break;
                    }
                };
            }
            TMLogger.info('  Based on layout: ' + mySection.tmLayoutName());
            var myStart = mySection.pageStart.documentOffset;
            var myCount = myStart + mySection.alternateLayoutLength;
            for (var i = myStart; i < myCount; i++) {
                mySpreads.push(myDocument.pages[i].parent);
            }
            return mySpreads;
        } catch (myException) {
            TMLogger.exception('TMAlternateLayouts.spreadsForDocument', myException + ' (tm_alternatelayout:285)');
            return [];
        }
    },
    baseForLayout: function (myDocumentLayouts, myCurrentLayout) {
        try {
            var mySectionBase = myCurrentLayout.substr(0, myCurrentLayout.length - 2);
            var currentOrientation = myCurrentLayout.substr(myCurrentLayout.length - 2, 2);
            var otherOrientation = (currentOrientation == ' H') ? ' V' : ' H';
            var fallbackMap = {
                'iPad': ['Android 10"', 'Kindle Fire/Nook'],
                'Android 10"': ['iPad', 'Kindle Fire/Nook'],
                'Kindle Fire/Nook': ['Android 10"', 'iPad'],
                'Phone': ['Kindle Fire/Nook', 'Android 10"', 'iPad'],
            };
            var fallbackLayouts = fallbackMap[mySectionBase];
            for (var i = 0; i < fallbackLayouts.length; i++) {
                var fallbackLayout = fallbackLayouts[i];
                if (TMUtilities.itemInArray(myDocumentLayouts, fallbackLayout + currentOrientation)) {
                    return fallbackLayout + currentOrientation;
                }
                if (TMUtilities.itemInArray(myDocumentLayouts, mySectionBase + otherOrientation)) {
                    return mySectionBase + otherOrientation;
                }
                if (TMUtilities.itemInArray(myDocumentLayouts, fallbackLayout + otherOrientation)) {
                    return fallbackLayout + otherOrientation;
                }
            };
            return undefined;
        } catch (myException) {
            TMLogger.exception('TMAlternateLayouts.baseForLayout', myException + ' (tm_alternatelayout:320)');
            return undefined;
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TwixlPublisherPluginAPI = {
    checkIfInstalledAndLoaded: function () {
        try {
            version = TMVersion.version;
            return true;
        } catch (e) {
            TMDialogs.error('Twixl Publisher plugin is not loaded');
            return false;
        }
    },
    setExportFormat: function (exportFormat) {
        if (exportFormat.toLowerCase() == 'jpg') {
            kEXPORT_FORMAT = 'JPG';
        } else {
            kEXPORT_FORMAT = 'PDF';
        }
    },
    preflightPublication: function (publicationPath) {
        TMLogger.info('API: preflightPublication: publicationPath=' + publicationPath);
        return TMPreflighter.preflight([publicationPath]);
    },
    preflightArticle: function (articlePath) {
        TMLogger.info('API: preflightArticle: articlePath=' + articlePath);
        return TMPreflighter.preflightArticle([articlePath]);
    },
    preflightWarnings: function () {
        TMLogger.info('API: preflightWarnings');
        return TMJSON.stringify(TMPreflighter.warnings);
    },
    preflightErrors: function () {
        TMLogger.info('API: preflightErrors');
        return TMJSON.stringify(TMPreflighter.errors);
    },
    preflightWarningCount: function () {
        TMLogger.info('API: preflightWarningCount');
        return TMPreflighter.warningCount();
    },
    preflightErrorCount: function () {
        TMLogger.info('API: preflightErrorCount');
        return TMPreflighter.errorCount();
    },
    exportArticleInternal: function (args) {
        return TMExporter.exportArticle(args[0], args[1], {}, false);
    },
    exportArticle: function (articlePath, outputPath, closeProgress) {
        TMLogger.info('API: exportArticle: articlePath=' + articlePath + ', outputPath=' + outputPath);
        try {
            var preflightResult = TwixlPublisherPluginAPI.preflightArticle(articlePath);
            if (TwixlPublisherPluginAPI.preflightErrorCount() > 0) {
                TMLogger.error("Cannot export because there are preflight errors");
                return false;
            }
            return TMExporter.exportArticle(articlePath, outputPath, {}, true);
        } catch (myException) {
            $.writeln(myException);
            return false;
        }
    },
    exportPublication: function (publicationPath, outputPath) {
        TMLogger.info('API: exportPublication: publicationPath=' + publicationPath + ', outputPath=' + outputPath);
        try {
            var preflightResult = TwixlPublisherPluginAPI.preflightPublication(publicationPath);
            if (TwixlPublisherPluginAPI.preflightErrorCount() > 0) {
                TMLogger.error("Cannot export because there are preflight errors");
                return false;
            }
            return TMExporter.exportPublication(publicationPath, outputPath, {});
        } catch (myException) {
            $.writeln(myException);
            return false;
        }
    },
    uploadPublicationToWoodwing: function (publicationPath, server) {
        TMLogger.info('API: uploadPublicationToWoodwing: publicationPath=' + publicationPath + ', server=' + server);
        return this.uploadArticleToWoodwing(publicationPath, server);
    },
    uploadResourcesToWoodwing: function (resourcesPath, server, basedir, extraParams) {
        TMLogger.info('API: uploadResourcesToWoodwing: resourcesPath=' + resourcesPath + ', server=' + server + ', basedir=' + basedir + ', extraParams=' + extraParams);
        params = {};
        params['action'] = 'TMWoodwingResourcesUploader';
        params['server'] = server;
        params['basedir'] = basedir;
        params['params'] = extraParams;
        return TMTaskQueue.addTask(resourcesPath, params);
    },
    uploadArticleToWoodwing: function (articlePath, server, basedir, extraParams) {
        TMLogger.info('API: uploadArticleToWoodwing: articlePath=' + articlePath + ', server=' + server + ', basedir=' + basedir + ', extraParams=' + extraParams);
        params = {};
        params['action'] = 'TMWoodwingUploader';
        params['server'] = server;
        params['basedir'] = basedir;
        params['params'] = extraParams;
        return TMTaskQueue.addTask(articlePath, params);
    },
    previewPublicationOnDevice: function (publicationPath, server) {
        TMLogger.info('API: previewPublicationOnDevice: publicationPath=' + publicationPath + ', server=' + server);
        return this.previewArticleOnDevice(publicationPath, server);
    },
    previewArticleOnDevice: function (articlePath, server, extraParams) {
        TMLogger.info('API: previewArticleOnDevice: articlePath=' + articlePath + ', server=' + server + ', extraParams=' + extraParams);
        params = {};
        params['action'] = 'TMPublicationPreviewer';
        params['server'] = server;
        params['params'] = extraParams;
        params['deviceMode'] = TMPreferences.readObject('preview.deviceMode');
        params['deviceModel'] = TMPreferences.readObject('preview.deviceModel');
        return TMTaskQueue.addTask(articlePath, params);
    },
    sharePublication: function (publicationPath, from, to, message) {
        TMLogger.info('API: sharePublication: publicationPath=' + publicationPath + ', from=' + from + ', to=' + to, +', message=' + message);
        TMBridge.tmShare([from, to, message]);
    },
    shareArticle: function (articlePath, from, to, message) {
        TMLogger.info('API: shareArticle: articlePath=' + articlePath + ', from=' + from + ', to=' + to, +', message=' + message);
        TMBridge.tmShare([from, to, message, articlePath]);
    },
    exportSupportLogs: function (exportPath) {
        TMLogger.info('API: exportSupportLogs: exportPath=' + exportPath);
        params = {};
        params['action'] = 'TMExportSupportLogs';
        params['path'] = exportPath;
        params['path_documents'] = Folder.myDocuments.fsName;
        params['path_user_data'] = Folder.userData.fsName;
        return TMTaskQueue.addTask(undefined, params);
    },
    compressZip: function (srcPath, dstFile) {
        return TMHelper.call('zip/compress', {
            'path': srcPath,
            'to': dstFile
        });
    },
    decompressZip: function (srcFile, dstPath) {
        return TMHelper.call('zip/decompress', {
            'file': srcFile,
            'to': dstPath
        });
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMApplication = {
    appSelection: function () {
        try {
            if (app.selection.length == 1) {
                return app.selection[0];
            }
        } catch (myException) { }
        return undefined;
    },
    isServer: function () {
        return app.name == 'Adobe InDesign Server';
    },
    deselectAllObjects: function () {
        try {
            app.selection = NothingEnum.NOTHING;
        } catch (myException) { }
    },
    currentArticleName: function () {
        try {
            return app.activeDocument.name;
        } catch (myException) {
            TMStackTrace.addToStack("TMApplication.currentArticleName", myException);
        }
    },
    currentPublicationName: function () {
        try {
            return app.activeBook.name;
        } catch (myException) {
            return 'No Publication Open';
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
Behavior.prototype.tmParentPage = function () {
    try {
        return TMPageItem.parentPage(this.parent);
    } catch (myException) {
        TMLogger.exception("Behavior.prototype.tmParentPage", myException + ' (tm_behavior:10)');
        return undefined;
    }
};
Behavior.prototype.tmParentDocument = function () {
    try {
        return this.tmParentPage().tmParentDocument();
    } catch (myException) {
        TMLogger.exception("Behavior.prototype.tmParentDocument", myException + ' (tm_behavior:19)');
        return undefined;
    }
};
Behavior.prototype.tmArticleName = function () {
    try {
        return this.tmParentDocument().tmArticleName();
    } catch (myException) {
        TMLogger.exception("Behavior.prototype.tmArticleName", myException + ' (tm_behavior:28)');
        return "";
    }
};
Behavior.prototype.tmProperties = function (myParentPage) {
    try {
        var myAction = undefined;
        if (this instanceof GotoAnchorBehavior) {
            var myFile = undefined;
            if (!(this.filePath instanceof File)) {
                myFile = new File(this.filePath);
            }
            try {
                if (this.properties.hasOwnProperty('anchorItem')) {
                    if (this.anchorItem instanceof HyperlinkPageDestination) {
                        var myPage = this.anchorItem.destinationPage.tmRelativePageNumber();
                        var myArticle = this.anchorItem.parent.tmArticleName();
                        if (myPage > 0) {
                            myAction = {
                                'action': 'pagelink',
                                'page': myPage,
                                'article': myArticle,
                            };
                        }
                    } else if (this.anchorItem instanceof HyperlinkTextDestination) {
                        var myPage = this.anchorItem.destinationText.parentTextFrames[0].parentPage.tmRelativePageNumber();
                        var myArticle = this.anchorItem.parent.tmArticleName();
                        if (myPage > 0) {
                            myAction = {
                                'action': 'pagelink',
                                'page': myPage,
                                'article': myArticle,
                            };
                        }
                    }
                }
                if (this.properties.hasOwnProperty('anchorName')) {
                    var parentDocument = this.tmParentDocument();
                    var anchorName = this.properties.anchorName;
                    var bookmark = parentDocument.bookmarks.itemByName(anchorName);
                    if (bookmark.isValid) {
                        var destination = parentDocument.bookmarks.itemByName(anchorName).destination;
                        if (destination.isValid) {
                            if (destination instanceof HyperlinkPageDestination) {
                                var myPage = destination.destinationPage.tmRelativePageNumber();
                                var myArticle = destination.parent.tmArticleName();
                                if (myPage > 0) {
                                    myAction = {
                                        'action': 'pagelink',
                                        'page': myPage,
                                        'article': myArticle,
                                    };
                                }
                            } else if (destination instanceof HyperlinkTextDestination) {
                                var myPage = destination.destinationText.parentTextFrames[0].parentPage.tmRelativePageNumber();
                                var myArticle = destination.parent.tmArticleName();
                                if (myPage > 0) {
                                    myAction = {
                                        'action': 'pagelink',
                                        'page': myPage,
                                        'article': myArticle,
                                    };
                                }
                            }
                        }
                    }
                }
            } catch (myException) {
                TMLogger.exception("Behavior.prototype.tmProperties/GotoAnchorBehavior", myException + ' (tm_behavior:113)');
            }
            if (myAction == undefined && myFile != undefined) {
                myAction = {
                    'action': 'pagelink',
                    'page': 1,
                    'article': myFile.tmArticleName(),
                };
                if (myAction['article'] == undefined) {
                    myAction['article'] = "";
                }
            }
        }
        if (this instanceof GotoStateBehavior) {
            try {
                if (this.associatedMultiStateObject != undefined) {
                    myAction = {
                        'action': 'slideshow',
                        'option': 'go_to_state',
                        'slideshow': this.associatedMultiStateObject.id,
                        'slide': this.stateName,
                    };
                }
            } catch (myException) {
                TMLogger.exception("Behavior.prototype.tmProperties/GotoStateBehavior", myException + ' (tm_behavior:143)');
            }
        }
        if (this instanceof GotoPreviousStateBehavior) {
            try {
                if (this.associatedMultiStateObject != undefined) {
                    myAction = {
                        'action': 'slideshow',
                        'option': 'go_to_previous_state',
                        'slideshow': this.associatedMultiStateObject.id,
                        'loop': this.loopsToNextOrPrevious ? "yes" : "no",
                    };
                }
            } catch (myException) {
                TMLogger.exception("Behavior.prototype.tmProperties/GotoPreviousStateBehavior", myException + ' (tm_behavior:161)');
            }
        }
        if (this instanceof GotoNextStateBehavior) {
            try {
                if (this.associatedMultiStateObject != undefined) {
                    myAction = {
                        'action': 'slideshow',
                        'option': 'go_to_next_state',
                        'slideshow': this.associatedMultiStateObject.id,
                        'loop': this.loopsToNextOrPrevious ? "yes" : "no",
                    };
                }
            } catch (myException) {
                TMLogger.exception("Behavior.prototype.tmProperties/GotoNextStateBehavior", myException + ' (tm_behavior:179)');
            }
        }
        if (this instanceof GotoPageBehavior) {
            try {
                myAction = {
                    'action': 'pagelink',
                    'page': this.pageNumber,
                    'article': this.tmArticleName(),
                };
                myAction = TMGeometry.addCorrectedBounds(myAction, this.parent.visibleBounds);
            } catch (myException) {
                TMLogger.exception("Behavior.prototype.tmProperties/GotoPageBehavior", myException + ' (tm_behavior:193)');
            }
        }
        if (this instanceof GotoPreviousPageBehavior) {
            try {
                myAction = {
                    'action': 'goto_previous_page',
                };
                myAction = TMGeometry.addCorrectedBounds(myAction, this.parent.visibleBounds);
            } catch (myException) {
                TMLogger.exception("Behavior.prototype.tmProperties/GotoPreviousPageBehavior", myException + ' (tm_behavior:205)');
            }
        }
        if (this instanceof GotoNextPageBehavior) {
            try {
                myAction = {
                    'action': 'goto_next_page',
                };
                myAction = TMGeometry.addCorrectedBounds(myAction, this.parent.visibleBounds);
            } catch (myException) {
                TMLogger.exception("Behavior.prototype.tmProperties/GotoNextPageBehavior", myException + ' (tm_behavior:217)');
            }
        }
        if (this instanceof GotoFirstPageBehavior) {
            try {
                myAction = {
                    'action': 'goto_first_page',
                };
                myAction = TMGeometry.addCorrectedBounds(myAction, this.parent.visibleBounds);
            } catch (myException) {
                TMLogger.exception("Behavior.prototype.tmProperties/GotoFirstPageBehavior", myException + ' (tm_behavior:229)');
            }
        }
        if (this instanceof GotoLastPageBehavior) {
            try {
                myAction = {
                    'action': 'goto_last_page',
                };
                myAction = TMGeometry.addCorrectedBounds(myAction, this.parent.visibleBounds);
            } catch (myException) {
                TMLogger.exception("Behavior.prototype.tmProperties/GotoLastPageBehavior", myException + ' (tm_behavior:241)');
            }
        }
        if (this instanceof GotoURLBehavior) {
            try {
                var url = TMUtilities.normalizeUrl(this.url);
                if (url != undefined) {
                    myAction = {
                        'action': 'weblink',
                        'url': url,
                    };
                    myAction = TMGeometry.addCorrectedBounds(myAction, this.parent.visibleBounds);
                }
            } catch (myException) {
                TMLogger.exception("Behavior.prototype.tmProperties/GotoURLBehavior", myException + ' (tm_behavior:257)');
            }
        }
        if (this instanceof MovieBehavior) {
            try {
                if (this.operation.toString().toLowerCase() != 'play_from_navigation_point') {
                    if (this.movieItem != undefined) {
                        myAction = {
                            'action': 'movie',
                            'option': this.operation.toString().toLowerCase(),
                            'movie': this.movieItem.id,
                        }
                        if (myAction['option'] == 'stop_all') {
                            myAction['option'] = 'stop';
                        }
                    }
                }
            } catch (myException) {
                TMLogger.exception("Behavior.prototype.tmProperties/MovieBehavior", myException + ' (tm_behavior:277)');
            }
        }
        if (this instanceof SoundBehavior) {
            try {
                if (this.soundItem != undefined) {
                    myAction = {
                        'action': 'sound',
                        'option': this.operation.toString().toLowerCase(),
                        'sound': this.soundItem.id,
                    };
                    if (myAction['option'] == 'stop_all') {
                        myAction['option'] = 'stop';
                    }
                }
            } catch (myException) {
                TMLogger.exception("Behavior.prototype.tmProperties/SoundBehavior", myException + ' (tm_behavior:295)');
            }
        }
        try {
            myAction["id"] = this.id;
        } catch (myException) {
            TMLogger.exception("Behavior.prototype.tmProperties/ID", myException + ' (tm_behavior:302)');
        }
        if (this.behaviorEvent == BehaviorEvents.MOUSE_UP || this.behaviorEvent == BehaviorEvents.MOUSE_DOWN) {
            return myAction;
        }
    } catch (myException) {
        TMLogger.exception("Behavior.prototype.tmProperties", myException + ' (tm_behavior:311)');
        return {};
    }
};
GotoAnchorBehavior.prototype.tmProperties = Behavior.prototype.tmProperties;
GotoPageBehavior.prototype.tmProperties = Behavior.prototype.tmProperties;
GotoPreviousPageBehavior.prototype.tmProperties = Behavior.prototype.tmProperties;
GotoNextPageBehavior.prototype.tmProperties = Behavior.prototype.tmProperties;
GotoFirstPageBehavior.prototype.tmProperties = Behavior.prototype.tmProperties;
GotoLastPageBehavior.prototype.tmProperties = Behavior.prototype.tmProperties;
GotoStateBehavior.prototype.tmProperties = Behavior.prototype.tmProperties;
GotoPreviousStateBehavior.prototype.tmProperties = Behavior.prototype.tmProperties;
GotoNextStateBehavior.prototype.tmProperties = Behavior.prototype.tmProperties;
GotoURLBehavior.prototype.tmProperties = Behavior.prototype.tmProperties;
MovieBehavior.prototype.tmProperties = Behavior.prototype.tmProperties;
SoundBehavior.prototype.tmProperties = Behavior.prototype.tmProperties;
GotoAnchorBehavior.prototype.tmParentPage = Behavior.prototype.tmParentPage;
GotoPageBehavior.prototype.tmParentPage = Behavior.prototype.tmParentPage;
GotoPreviousPageBehavior.prototype.tmParentPage = Behavior.prototype.tmParentPage;
GotoNextPageBehavior.prototype.tmParentPage = Behavior.prototype.tmParentPage;
GotoFirstPageBehavior.prototype.tmParentPage = Behavior.prototype.tmParentPage;
GotoLastPageBehavior.prototype.tmParentPage = Behavior.prototype.tmParentPage;
GotoStateBehavior.prototype.tmParentPage = Behavior.prototype.tmParentPage;
GotoPreviousStateBehavior.prototype.tmParentPage = Behavior.prototype.tmParentPage;
GotoNextStateBehavior.prototype.tmParentPage = Behavior.prototype.tmParentPage;
GotoURLBehavior.prototype.tmParentPage = Behavior.prototype.tmParentPage;
MovieBehavior.prototype.tmParentPage = Behavior.prototype.tmParentPage;
SoundBehavior.prototype.tmParentPage = Behavior.prototype.tmParentPage;
GotoAnchorBehavior.prototype.tmParentDocument = Behavior.prototype.tmParentDocument;
GotoPageBehavior.prototype.tmParentDocument = Behavior.prototype.tmParentDocument;
GotoPreviousPageBehavior.prototype.tmParentDocument = Behavior.prototype.tmParentDocument;
GotoNextPageBehavior.prototype.tmParentDocument = Behavior.prototype.tmParentDocument;
GotoFirstPageBehavior.prototype.tmParentDocument = Behavior.prototype.tmParentDocument;
GotoLastPageBehavior.prototype.tmParentDocument = Behavior.prototype.tmParentDocument;
GotoStateBehavior.prototype.tmParentDocument = Behavior.prototype.tmParentDocument;
GotoPreviousStateBehavior.prototype.tmParentDocument = Behavior.prototype.tmParentDocument;
GotoNextStateBehavior.prototype.tmParentDocument = Behavior.prototype.tmParentDocument;
GotoURLBehavior.prototype.tmParentDocument = Behavior.prototype.tmParentDocument;
MovieBehavior.prototype.tmParentDocument = Behavior.prototype.tmParentDocument;
SoundBehavior.prototype.tmParentDocument = Behavior.prototype.tmParentDocument;
GotoAnchorBehavior.prototype.tmArticleName = Behavior.prototype.tmArticleName;
GotoPageBehavior.prototype.tmArticleName = Behavior.prototype.tmArticleName;
GotoPreviousPageBehavior.prototype.tmArticleName = Behavior.prototype.tmArticleName;
GotoNextPageBehavior.prototype.tmArticleName = Behavior.prototype.tmArticleName;
GotoFirstPageBehavior.prototype.tmArticleName = Behavior.prototype.tmArticleName;
GotoLastPageBehavior.prototype.tmArticleName = Behavior.prototype.tmArticleName;
GotoStateBehavior.prototype.tmArticleName = Behavior.prototype.tmArticleName;
GotoPreviousStateBehavior.prototype.tmArticleName = Behavior.prototype.tmArticleName;
GotoNextStateBehavior.prototype.tmArticleName = Behavior.prototype.tmArticleName;
GotoURLBehavior.prototype.tmArticleName = Behavior.prototype.tmArticleName;
MovieBehavior.prototype.tmArticleName = Behavior.prototype.tmArticleName;
SoundBehavior.prototype.tmArticleName = Behavior.prototype.tmArticleName;
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
Book.prototype.tmRequiredOrientations = function () {
    try {
        if (TMBook.tmRequiredOrientations == undefined) {
            var myMetaData = this.tmProperties();
            TMBook.tmRequiredOrientations = myRequiredOrientations = ['portrait', 'landscape'];
            if (myMetaData['orientations'] == 1) {
                TMBook.tmRequiredOrientations = myRequiredOrientations = ['portrait'];
            }
            if (myMetaData['orientations'] == 0) {
                TMBook.tmRequiredOrientations = myRequiredOrientations = ['landscape'];
            }
        }
        return TMBook.tmRequiredOrientations;
    } catch (myException) {
        TMLogger.exception("Book.prototype.tmRequiredOrientations", myException + ' (tm_book:20)');
        return [];
    }
};
Book.prototype.tmUsesAlternateLayouts = function () {
    try {
        if (TMPreferences.isRunningLegacyMode() == false) {
            return true;
        }
        if (TMBook.tmUsesAlternateLayouts == undefined) {
            var myProps = this.tmProperties();
            TMBook.tmUsesAlternateLayouts = myProps['alternateLayouts'];
        }
        return TMBook.tmUsesAlternateLayouts;
    } catch (myException) {
        TMLogger.silentException("Book.prototype.tmUsesAlternateLayouts", myException);
        return false;
    }
};
Book.prototype.tmIsSupportedOnDevice = function (myDeviceType) {
    try {
        if (TMBook.tmIsSupportedOnDevice == undefined) {
            if (this.tmUsesAlternateLayouts() && myDeviceType.tmStartsWith('ipad')) {
                TMBook.tmIsSupportedOnDevice = false;
                var myLayouts = TMUtilities.objectKeys(TMAlternateLayouts.publicationLayouts(this, false));
                for (var i in myLayouts) {
                    if (myLayouts[i].toLowerCase().tmStartsWith('ipad') == true) {
                        TMBook.tmIsSupportedOnDevice = true;
                        break;
                    }
                }
            } else {
                TMBook.tmIsSupportedOnDevice = true;
            }
        }
        return TMBook.tmIsSupportedOnDevice;
    } catch (myException) {
        TMLogger.exception("Book.prototype.tmIsSupportedOnDevice", myException + ' (tm_book:59)');
        return false;
    }
}
Book.prototype.tmShowStatusBar = function () {
    try {
        if (TMPreferences.isRunningLegacyMode() == false) {
            return false;
        }
        if (TMBook.tmShowStatusBar == undefined) {
            var myProps = this.tmProperties();
            TMBook.tmShowStatusBar = myProps['showStatusBar'] != false;
        }
        return TMBook.tmShowStatusBar;
    } catch (myException) {
        TMLogger.exception("Book.prototype.tmShowStatusBar", myException + ' (tm_book:75)');
        return false;
    }
};
Book.prototype.tmExportQuality = function () {
    try {
        if (TMBook.tmExportQuality == undefined) {
            TMBook.tmExportQuality = 'high';
            var myProps = this.tmProperties();
            if (myProps.hasOwnProperty('jpegQuality')) {
                var myQuality = myProps['jpegQuality'].toLowerCase();
                if (myQuality == 'maximum' || myQuality == 'medium' || myQuality == 'low') {
                    TMBook.tmExportQuality = myQuality;
                }
            }
        }
        return TMBook.tmExportQuality;
    } catch (myException) {
        TMLogger.exception("Book.prototype.tmExportQuality", myException + ' (tm_book:94)');
        return 'high';
    }
};
Book.prototype.tmShowScrubber = function () {
    try {
        if (TMBook.tmShowScrubber == undefined) {
            var myProps = this.tmProperties();
            TMBook.tmShowScrubber = myProps['scrubber'] != false;
        }
        return TMBook.tmShowScrubber;
    } catch (myException) {
        TMLogger.exception("Book.prototype.tmShowStatusBar", myException + ' (tm_book:107)');
        return true;
    }
};
Book.prototype.tmFiles = function () {
    try {
        if (TMBook.tmFiles == undefined) {
            var myBookFiles = [];
            var myBookContents = TMUtilities.collectionToArray(this.bookContents);
            var myBookContentsCount = myBookContents.length;
            for (var i = 0; i < myBookContentsCount; i++) {
                var myFile = myBookContents[i];
                myBookFiles.push(myBookContent.fullName.fsName);
            }
            TMBook.tmFiles = myBookFiles;
        }
        return TMBook.tmFiles;
    } catch (myException) {
        TMLogger.exception("Book.prototype.tmFiles", myException + ' (tm_book:126)');
        return [];
    }
};
Book.prototype.tmArticleNames = function () {
    try {
        if (TMBook.tmArticleNames == undefined) {
            var myArticleNames = [];
            var myBookContents = TMUtilities.collectionToArray(this.bookContents);
            var myBookContentsCount = myBookContents.length;
            for (var i = 0; i < myBookContentsCount; i++) {
                var myFile = myBookContents[i];
                myArticleNames.push(myFile.tmArticleName());
            }
            TMBook.tmArticleNames = TMUtilities.uniqueArrayValues(myArticleNames);
        }
        return TMBook.tmArticleNames;
    } catch (myException) {
        TMLogger.exception("Book.prototype.tmArticleNames", myException + ' (tm_book:145)');
        return {};
    }
};
Book.prototype.tmArticleCount = function () {
    try {
        if (TMBook.tmArticleCount == undefined) {
            TMBook.tmArticleCount = this.tmArticleNames().length;
        }
        return TMBook.tmArticleCount;
    } catch (myException) {
        TMLogger.exception("Book.prototype.tmArticleCount", myException + ' (tm_book:157)');
        return 0;
    }
};
Book.prototype.tmFileCount = function () {
    try {
        if (TMBook.tmFileCount == undefined) {
            TMBook.tmFileCount = this.bookContents.count();
        }
        return TMBook.tmFileCount;
    } catch (myException) {
        TMLogger.exception("Book.prototype.tmFileCount", myException + ' (tm_book:169)');
        return 0;
    }
};
Book.prototype.tmBookFiles = function () {
    try {
        if (TMBook.tmBookFiles == undefined) {
            var myBookFiles = [];
            var myBookContents = TMUtilities.collectionToArray(this.bookContents);
            var myBookContentsCount = myBookContents.length;
            for (var i = 0; i < myBookContentsCount; i++) {
                var myFile = myBookContents[i];
                myBookFiles.push(TMFiles.getBaseName(myFile.fullName.name));
            }
            TMBook.tmBookFiles = myBookFiles;
        }
        return TMBook.tmBookFiles;
    } catch (myException) {
        TMLogger.exception("Book.prototype.tmBookFiles", myException + ' (tm_book:188)');
        return [];
    }
};
Book.prototype.tmBookFilePaths = function () {
    try {
        if (TMBook.tmBookFilePaths == undefined) {
            var myBookFiles = [];
            var myBookContents = TMUtilities.collectionToArray(this.bookContents);
            var myBookContentsCount = myBookContents.length;
            for (var i = 0; i < myBookContentsCount; i++) {
                var myFile = myBookContents[i];
                var myPath = myFile.filePath.fsName + '/' + TMFiles.getBaseName(myFile.name, true);
                myBookFiles.push(myPath);
            }
            TMBook.tmBookFilePaths = myBookFiles;
        }
        return TMBook.tmBookFilePaths;
    } catch (myException) {
        TMLogger.exception("Book.prototype.tmBookFilePaths", myException + ' (tm_book:208)');
        return [];
    }
};
Book.prototype.tmPublicationStyle = function () {
    try {
        var properties = this.tmProperties();
        if (properties['alternateLayouts']) {
            return 'Use Liquid and Alternate Layouts';
        } else if (properties['orientations'] == 2) {
            return 'Publication supports landscape and portrait (legacy)';
        } else if (properties['orientations'] == 1) {
            return 'Publication supports portrait only (legacy)';
        } else if (properties['orientations'] == 0) {
            return 'Publication supports landscape only (legacy)';
        }
    } catch (myException) {
        TMLogger.exception("Book.prototype.tmPublicationStyle", myException + ' (tm_book:226)');
        return 'Use Liquid and Alternate Layouts';
    }
};
Book.prototype.tmOrientations = function () {
    try {
        var properties = this.tmProperties();
        if (properties['alternateLayouts']) {
            return undefined;
        } else if (properties['orientations'] == 2) {
            return 'portrait+landscape';
        } else if (properties['orientations'] == 1) {
            return 'portrait';
        } else if (properties['orientations'] == 0) {
            return 'landscape';
        }
    } catch (myException) {
        TMLogger.exception("Book.prototype.tmOrientations", myException + ' (tm_book:244)');
        return undefined;
    }
}
Book.prototype.tmProperties = function () {
    try {
        var myMetaData = undefined;
        try {
            var myMetaData = eval(this.label.replace(/\r\n/g, '').replace(/\n/g, '').replace(/\r/g, ''));
        } catch (myException) {
            TMLogger.exception('Book.prototype.tmProperties', myException + ' (tm_book:255)');
        }
        if (myMetaData && !myMetaData.hasOwnProperty('showStatusBar')) {
            myMetaData['showStatusBar'] = false;
        }
        if (myMetaData && !myMetaData.hasOwnProperty('alternateLayouts')) {
            myMetaData['alternateLayouts'] = true;
        }
        if (myMetaData && !myMetaData.hasOwnProperty('jpegQuality')) {
            myMetaData['jpegQuality'] = 'High';
        }
        if (myMetaData && !myMetaData.hasOwnProperty('allowSharing')) {
            myMetaData['allowSharing'] = 0;
        }
        if (myMetaData && !myMetaData.hasOwnProperty('lastTemplate')) {
            myMetaData['lastTemplate'] = 'iPad Landscape';
        }
        if (myMetaData && myMetaData.hasOwnProperty('lastTemplate') && myMetaData['lastTemplate'] == '') {
            myMetaData['lastTemplate'] = 'iPad Landscape';
        }
        if (!myMetaData) {
            myMetaData = {
                name: '',
                number: '',
                fullScreenSlideshows: true,
                showStatusBar: false,
                alternateLayouts: true,
                jpegQuality: 'High',
                allowSharing: 1,
                lastTemplate: 'iPad Landscape',
            };
        }
        return myMetaData;
    } catch (myException) {
        TMLogger.exception("Book.prototype.tmProperties", myException + ' (tm_book:289)');
        return {};
    }
};
Book.prototype.tmCreateArticle = function (myName, myTitle, myTagLine, myArticleAuthor, myArticleUrl, myOrientation, myTemplateName, myShowInScrubber, myShowStatusBar, myAlternateLayouts, myPlaylist) {
    try {
        var mySuffix = '';
        var myDocName = myName;
        if (myOrientation != undefined) {
            mySuffix = (myShowStatusBar) ? '_statusbar' : '_nostatusbar';
            mySuffix += (myOrientation == 'landscape' ? '_Ls' : '_Pt');
            myDocName += (myOrientation == 'landscape' ? '_Ls' : '_Pt');
        }
        var myDocRoot = this.filePath.fsName;
        var myDocPath = new File(myDocRoot + '/' + myDocName + '.indd');
        TMLogger.info('Creating Article: ' + myDocName);
        if (myDocPath.exists) {
            TMLogger.error('Failed to create document, exists already: ' + myDocPath);
            TMDialogs.error('Document exists already:\n' + TMUtilities.decodeURI(myDocPath.fsName));
            return;
        }
        var myTemplateFolderName = "Templates";
        if (myAlternateLayouts) {
            myTemplateFolderName = "Templates using Alternate Layout";
        }
        if (File.fs == "Macintosh") {
            var myTemplateRootPath1 = new File('/Library/Application Support/Twixl Publisher Plugin/' + myTemplateFolderName + '/' + myTemplateName + mySuffix + '.idml');
            var myTemplateUserPath1 = new File('~/Library/Application Support/Twixl Publisher Plugin/' + myTemplateFolderName + '/' + myTemplateName + mySuffix + '.idml');
            var myTemplateRootPath2 = new File('/Library/Application Support/Twixl Publisher Plugin/' + myTemplateFolderName + '/' + myTemplateName + mySuffix + '.indt');
            var myTemplateUserPath2 = new File('~/Library/Application Support/Twixl Publisher Plugin/' + myTemplateFolderName + '/' + myTemplateName + mySuffix + '.indt');
        } else {
            var myTemplateRootPath1 = new File(Folder.appData + '/Twixl Publisher/' + myTemplateFolderName + '/' + myTemplateName + mySuffix + '.idml');
            var myTemplateUserPath1 = new File(Folder.myDocuments + '/Twixl Publisher/' + myTemplateFolderName + '/' + myTemplateName + mySuffix + '.idml');
            var myTemplateRootPath2 = new File(Folder.appData + '/Twixl Publisher/' + myTemplateFolderName + '/' + myTemplateName + mySuffix + '.indt');
            var myTemplateUserPath2 = new File(Folder.myDocuments + '/Twixl Publisher/' + myTemplateFolderName + '/' + myTemplateName + mySuffix + '.indt');
        }
        if (myTemplateUserPath1.exists) {
            myTemplatePath = myTemplateUserPath1;
        } else if (myTemplateUserPath2.exists) {
            myTemplatePath = myTemplateUserPath2;
        } else if (myTemplateRootPath1.exists) {
            myTemplatePath = myTemplateRootPath1;
        } else if (myTemplateRootPath2.exists) {
            myTemplatePath = myTemplateRootPath2;
        } else {
            TMLogger.error('Failed to find template: ' + myTemplateName);
            TMDialogs.error('Failed to find template: ' + myTemplateName);
            return;
        }
        TMLogger.info('Using template: ' + myTemplatePath.fsName);
        var myDocument = app.open(myTemplatePath, true);
        var publicationProperties = this.tmProperties();
        publicationProperties['lastTemplate'] = myTemplateName
        app.activeBook.label = publicationProperties.toSource();
        myDocument.metadataPreferences.documentTitle = myTitle;
        var myMetaData = {
            title: myTitle,
            tagline: myTagLine,
            author: myArticleAuthor,
            articleURL: myArticleUrl,
            showInScrubber: myShowInScrubber,
            backgroundMusicPlaylist: myPlaylist,
        }
        myDocument.label = myMetaData.toSource();
        myDocument.save(myDocPath);
        var myBookItem = this.bookContents.add(myDocPath);
        myDocument.sections.firstItem().continueNumbering = false;
        myDocument.sections.firstItem().pageNumberStart = 1;
        try {
            if (myDocument.filePath.readonly) {
                TMLogger.info('Failed to save the article: file is read-only');
            } else {
                myDocument.save();
            }
        } catch (myException) {
            TMLogger.error('Failed to save the article: ' + myException + ' (line ' + myException.line + ')');
        }
        myDocument.activeLayer = 'Base Layer'; // myDocument.layers.lastItem();
    } catch (myException) {
        TMLogger.exception("Book.prototype.tmCreateArticle", myException + ' (tm_book:399)');
    }
};
Book.prototype.tmSaveModifiedDocuments = function (myArticleName) {
    try {
        TMBook.resetCache();
        var myFiles = this.tmBookFiles();
        var myErrors = [];
        var myOpenDocuments = TMFiles.getVisibleDocuments();
        var myOpenDocumentsCount = myOpenDocuments.length;
        for (var i = 0; i < myOpenDocumentsCount; i++) {
            var myDoc = myOpenDocuments[i];
            if (myArticleName != undefined && myDoc.tmArticleName() != myArticleName) {
                continue;
            }
            if (!myDoc.saved) {
                continue;
            }
            if (!TMUtilities.itemInArray(myFiles, TMFiles.getReportName(myDoc.name))) {
                continue;
            }
            try {
                myDoc.save();
            } catch (myException) {
                TMLogger.error('Failed to automatically save the document: ' + myDoc.name);
            }
            if (myDoc.modified) {
                myErrors.push('File "' + myDoc.name + ' is not saved. Save the document before exporting.');
            }
        }
        return myErrors;
    } catch (myException) {
        TMLogger.exception("Book.prototype.tmSaveModifiedDocuments", myException + ' (tm_book:453)');
        return [];
    }
};
var TMBook = {
    tmRequiredOrientations: undefined,
    tmUsesAlternateLayouts: undefined,
    tmShowStatusBar: undefined,
    tmExportQuality: undefined,
    tmShowScrubber: undefined,
    tmFiles: undefined,
    tmArticleNames: undefined,
    tmArticleCount: undefined,
    tmFileCount: undefined,
    tmBookFiles: undefined,
    tmBookFilePaths: undefined,
    tmProperties: undefined,
    resetCache: function () {
        this.tmRequiredOrientations = undefined;
        this.tmUsesAlternateLayouts = undefined;
        this.tmShowStatusBar = undefined;
        this.tmExportQuality = undefined;
        this.tmShowScrubber = undefined;
        this.tmFiles = undefined;
        this.tmArticleNames = undefined;
        this.tmArticleCount = undefined;
        this.tmFileCount = undefined;
        this.tmBookFiles = undefined;
        this.tmBookFilePaths = undefined;
        this.tmProperties = undefined;
    },
}
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
BookContent.prototype.tmArticleName = function () {
    try {
        return TMFiles.getBaseName(this.name, true);
    } catch (myException) {
        TMLogger.exception("BookContent.prototype.tmArticleName", myException + ' (tm_bookcontent:10)');
        return 0;
    }
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMBridge = {
    /* OK */
    tmSaveBookProperties: function (name, style, scrubber, readingStyle, scrollEnabled, horizontalOnly, showStatusBar, jpegQuality, allowSharing) {
        var myOrientations = 0;
        var myAlternateLayouts = true;
        var myShowStatusBar = false;
        if (style == "Publication supports landscape only (legacy)") {
            myOrientations = 0;
            myAlternateLayouts = false;
            myShowStatusBar = showStatusBar;
        } else if (style == "Publication supports portrait only (legacy)") {
            myOrientations = 1;
            myAlternateLayouts = false;
            myShowStatusBar = showStatusBar;
        } else if (style == "Publication supports landscape and portrait (legacy)") {
            myOrientations = 2;
            myAlternateLayouts = false;
            myShowStatusBar = showStatusBar;
        }
        var bookProperties = app.activeBook.tmProperties();
        var lastTemplate = bookProperties['lastTemplate'];
        if (lastTemplate == '') {
            lastTemplate = 'iPad Landscape';
        }
        var myMetaData = {
            name: name,
            orientations: myOrientations,
            scrubber: scrubber ? 1 : 0,
            allowSharing: allowSharing ? 1 : 0,
            scrollEnabled: scrollEnabled ? 1 : 0,
            horizontalOnly: horizontalOnly ? 1 : 0,
            showStatusBar: myShowStatusBar ? 1 : 0,
            alternateLayouts: myAlternateLayouts,
            jpegQuality: jpegQuality,
            lastTemplate: lastTemplate,
            tm_version: TMVersion.version,
            tm_build: TMVersion.build,
            readingStyle: readingStyle,
        };
        app.activeBook.label = myMetaData.toSource();
        TMBook.resetCache();
    },
    /* OK */
    tmSaveArticleProperties: function (title, tagline, author, articleURL, showInScrubber, playlist) {
        var myMetaData = {
            title: title,
            tagline: tagline,
            author: author,
            articleURL: articleURL,
            showInScrubber: showInScrubber ? '1' : '0',
            backgroundMusicPlaylist: playlist,
            tm_version: TMVersion.version,
            tm_build: TMVersion.build,
        }
        app.activeDocument.label = myMetaData.toSource();
        try {
            if (app.activeDocument.filePath.readonly) {
                TMLogger.info('Failed to save the publication: file is read-only');
            } else {
                app.activeDocument.save();
                TMLogger.info('Saved article properties to: ' + app.activeDocument.filePath.fsName);
            }
        } catch (myException) {
            TMLogger.silentException('TMBridge.tmSaveArticleProperties', myException);
        }
        if (app.activeDocument.name.replace('.indd', '').replace('.indt', '').replace('.idml', '').match("_Pt$") == '_Pt') {
            var myOtherOrientation1 = TMFiles.getBaseName(app.activeDocument.fullName) + '_Ls.indd';
            var myOtherOrientation2 = TMFiles.getBaseName(app.activeDocument.fullName) + '_Ls';
        } else {
            var myOtherOrientation1 = TMFiles.getBaseName(app.activeDocument.fullName) + '_Pt.indd';
            var myOtherOrientation2 = TMFiles.getBaseName(app.activeDocument.fullName) + '_Pt';
        }
        var myOtherOrientation = undefined;
        var myOtherOrientationFile1 = new File(myOtherOrientation1);
        var myOtherOrientationFile2 = new File(myOtherOrientation2);
        if (myOtherOrientationFile1.exists) {
            myOtherOrientation = myOtherOrientationFile1;
        } else if (myOtherOrientationFile2.exists) {
            myOtherOrientation = myOtherOrientationFile2;
        }
        if (!myOtherOrientation) {
            return;
        }
        try {
            var myOpenFiles = TMFiles.getVisibleFiles();
            if (myDocument = app.open(new File(myOtherOrientation), false)) {
                var myMetaData = myDocument.tmProperties();
                myMetaData['showInScrubber'] = myDialog.attributes.options.showInScrubber.value ? '1' : '0';
                myDocument.label = myMetaData.toSource();
                try {
                    if (myDocument.filePath.readonly) {
                        TMLogger.info('Failed to save the article: file is read-only');
                    } else {
                        myDocument.save();
                        TMLogger.info('Saved article properties to: ' + app.activeDocument.filePath.fsName);
                    }
                } catch (myException) {
                    TMLogger.silentException('TMBridge.tmSaveArticleProperties', myException);
                }
                var fsName = myDocument.filePath.fsName + '/' + myDocument.name;
                if (!TMUtilities.itemInArray(myOpenFiles, fsName)) {
                    myDocument.close();
                }
            }
        } catch (myException) {
            TMLogger.silentException('TMBridge.tmSaveArticleProperties', myException);
        }
        TMBook.resetCache();
    },
    /* OK */
    tmNewPublication: function (pubName, style, showScrubber, readingStyle, allowSwiping, horizontalOnly, showStatusBar, articleName, articleTitle, articleTagLine, articleTemplate, allowSharing) {
        try {
            var publicationFileName = TMFiles.filterFileName(pubName);
            var myBookLocation = new File(Folder.desktop.fsName + "/" + publicationFileName + '.indb');
            myBookLocation = myBookLocation.saveDlg("Save new publication as...", "InDesign Books:*.indb;All files:*.*");
            if (myBookLocation) {
                var myPortrait = false;
                var myLandscape = false;
                var myAlternateLayouts = true;
                var myShowStatusBar = false;
                if (style == "Publication supports landscape only (legacy)") {
                    myLandscape = true;
                    myAlternateLayouts = false;
                    myShowStatusBar = showStatusBar;
                } else if (style == "Publication supports portrait only (legacy)") {
                    myPortrait = true;
                    myAlternateLayouts = false;
                    myShowStatusBar = showStatusBar;
                } else if (style == "Publication supports landscape and portrait (legacy)") {
                    myLandscape = true;
                    myPortrait = true;
                    myAlternateLayouts = false;
                    myShowStatusBar = showStatusBar;
                }
                var myArticleName = TMFiles.filterFileName(articleName);
                var hasArticle = myArticleName.length > 0;
                var numSteps = 1;
                if (hasArticle) {
                    if (myPortrait && myLandscape) {
                        numSteps = 3;
                    } else if ((myPortrait && !myLandscape) || (!myPortrait && myLandscape)) {
                        numSteps = 2;
                    } else if (myAlternateLayouts) {
                        numSteps = 2;
                    }
                }
                TMProgressBar.create(kAPP_NAME, 'Creating publication', numSteps);
                if (!myBookLocation.fsName.tmEndsWith('.indb')) {
                    myBookLocation = new File(myBookLocation.fsName + '.indb');
                }
                TMLogger.info('Creating Publication: ' + myBookLocation.fsName);
                var myBook = app.books.add(myBookLocation);
                var myHtmlResources = new Folder(myBook.filePath.fsName + "/WebResources");
                if (!myHtmlResources.exists) {
                    TMLogger.info('Creating: ' + myHtmlResources.fsName);
                    myHtmlResources.create();
                }
                var myMetaData = {
                    name: pubName,
                    orientations: 2,
                    horizontalOnly: horizontalOnly,
                    scrollEnabled: allowSwiping,
                    scrubber: showScrubber,
                    showStatusBar: myShowStatusBar,
                    alternateLayouts: myAlternateLayouts,
                    allowSharing: allowSharing,
                    readingStyle: readingStyle
                };
                if (myPortrait == true && myLandscape == false) {
                    myMetaData['orientations'] = 1;
                } else if (myPortrait == false && myLandscape == true) {
                    myMetaData['orientations'] = 0;
                }
                var myArticleTitle = articleTitle;
                var myArticleTagLine = articleTagLine;
                var myArticleAuthor = '';
                var myArticleUrl = '';
                var myTemplateName = articleTemplate;
                var myShowInScrubber = '1';
                var myShowStatusBar = myShowStatusBar;
                var myAlternateLayouts = myAlternateLayouts;
                myMetaData['lastTemplate'] = myTemplateName;
                myBook.label = myMetaData.toSource();
                if (myArticleName.length > 0) {
                    if (myAlternateLayouts) {
                        TMProgressBar.updateLabel('Creating article: ' + TMUtilities.decodeURI(myArticleName));
                        app.activeBook.tmCreateArticle(
                            myArticleName,
                            myArticleTitle,
                            myArticleTagLine,
                            myArticleAuthor,
                            myArticleUrl,
                            undefined,
                            myTemplateName,
                            myShowInScrubber,
                            myShowStatusBar,
                            myAlternateLayouts
                        );
                        TMProgressBar.updateProgressBySteps(1);
                    } else {
                        if (myLandscape) {
                            TMProgressBar.updateLabel('Creating article: ' + TMUtilities.decodeURI(myArticleName) + ' (landscape)');
                            app.activeBook.tmCreateArticle(
                                myArticleName,
                                myArticleTitle,
                                myArticleTagLine,
                                myArticleAuthor,
                                myArticleUrl,
                                'landscape',
                                myTemplateName,
                                myShowInScrubber,
                                myShowStatusBar,
                                myAlternateLayouts
                            );
                            TMProgressBar.updateProgressBySteps(1);
                        }
                        if (myPortrait) {
                            TMProgressBar.updateLabel('Creating article: ' + TMUtilities.decodeURI(myArticleName) + ' (portrait)');
                            app.activeBook.tmCreateArticle(
                                myArticleName,
                                myArticleTitle,
                                myArticleTagLine,
                                myArticleAuthor,
                                myArticleUrl,
                                'portrait',
                                myTemplateName,
                                myShowInScrubber,
                                myShowStatusBar,
                                myAlternateLayouts
                            );
                            TMProgressBar.updateProgressBySteps(1);
                        }
                    }
                }
                try {
                    TMProgressBar.updateLabel('Creating publication: ' + TMUtilities.decodeURI(myMetaData['name']));
                    if (myBook.filePath.readonly) {
                        TMLogger.error('Failed to save the publication: file is read-only');
                    } else {
                        myBook.save();
                    }
                    TMProgressBar.updateProgressBySteps(1);
                } catch (myException) {
                    TMLogger.exception('TMBridge.tmNewPublication', myException + ' (tm_bridge:294)');
                }
            }
        } catch (myException) {
            TMLogger.exception('TMBridge.tmNewPublication', myException + ' (tm_bridge:300)');
        }
        TMProgressBar.close();
    },
    /* OK */
    tmNewArticle: function (myArticleName, myArticleTitle, myArticleTagLine, myArticleAuthor, myArticleUrl, myTemplateName, myShowInScrubber, myAlternateLayouts, myShowStatusBar, myOrientations, myPlaylist) {
        try {
            myArticleName = TMFiles.filterFileName(myArticleName);
            if (myArticleName.length == 0) {
                TMDialogs.error("You should give at least an article name.");
                return;
            }
            var numSteps = 2;
            if (myAlternateLayouts) {
                numSteps = 2;
            } else if (myOrientations) {
                numSteps = 3;
            }
            TMProgressBar.create(kAPP_NAME, 'Creating articles', numSteps);
            if (myAlternateLayouts) {
                TMProgressBar.updateLabel('Creating article: ' + TMUtilities.decodeURI(myArticleName));
                app.activeBook.tmCreateArticle(
                    myArticleName,
                    myArticleTitle,
                    myArticleTagLine,
                    myArticleAuthor,
                    myArticleUrl,
                    undefined,
                    myTemplateName,
                    myShowInScrubber,
                    myShowStatusBar,
                    myAlternateLayouts,
                    myPlaylist
                );
                TMProgressBar.updateProgressBySteps(1);
            } else {
                if (myOrientations == "landscape" || myOrientations == "portrait+landscape") {
                    TMProgressBar.updateLabel('Creating article: ' + TMUtilities.decodeURI(myArticleName) + ' (landscape)');
                    app.activeBook.tmCreateArticle(
                        myArticleName,
                        myArticleTitle,
                        myArticleTagLine,
                        myArticleAuthor,
                        myArticleUrl,
                        'landscape',
                        myTemplateName,
                        myShowInScrubber,
                        myShowStatusBar,
                        myAlternateLayouts,
                        myPlaylist
                    );
                    TMProgressBar.updateProgressBySteps(1);
                }
                if (myOrientations == "portrait" || myOrientations == "portrait+landscape") {
                    TMProgressBar.updateLabel('Creating article: ' + TMUtilities.decodeURI(myArticleName) + ' (portrait)');
                    app.activeBook.tmCreateArticle(
                        myArticleName,
                        myArticleTitle,
                        myArticleTagLine,
                        myArticleAuthor,
                        myArticleUrl,
                        'portrait',
                        myTemplateName,
                        myShowInScrubber,
                        myShowStatusBar,
                        myAlternateLayouts,
                        myPlaylist
                    );
                    TMProgressBar.updateProgressBySteps(1);
                }
            }
        } catch (myException) {
            TMLogger.exception('TMBridge.tmNewArticle', myException + ' (tm_bridge:387)');
        }
        TMProgressBar.close();
    },
    /* OK */
    tmNewSingleArticle: function (myArticleName, myArticleTitle, myArticleTagLine, myArticleAuthor, myArticleUrl, myTemplateName, myShowInScrubber, myPlaylist) {
        try {
            myArticleName = TMFiles.filterFileName(myArticleName);
            if (myArticleName.length == 0) {
                TMDialogs.error("You should give at least an article name.");
                return;
            }
            var articleFileName = myArticleName;
            var myArticleLocation = new File(Folder.desktop.fsName + "/" + articleFileName + '.indd');
            myArticleLocation = myArticleLocation.saveDlg("Save new article as...", "InDesign Document:*.indd;All files:*.*");
            if (myArticleLocation) {
                TMProgressBar.create(kAPP_NAME, 'Creating article: ' + myArticleName, 2);
                var myTemplateFolderName = "Templates using Alternate Layout";
                if (File.fs == "Macintosh") {
                    var myTemplateRootPath1 = new File('/Library/Application Support/Twixl Publisher Plugin/' + myTemplateFolderName + '/' + myTemplateName + '.idml');
                    var myTemplateUserPath1 = new File('~/Library/Application Support/Twixl Publisher Plugin/' + myTemplateFolderName + '/' + myTemplateName + '.idml');
                    var myTemplateRootPath2 = new File('/Library/Application Support/Twixl Publisher Plugin/' + myTemplateFolderName + '/' + myTemplateName + '.indt');
                    var myTemplateUserPath2 = new File('~/Library/Application Support/Twixl Publisher Plugin/' + myTemplateFolderName + '/' + myTemplateName + '.indt');
                } else {
                    var myTemplateRootPath1 = new File(Folder.appData + '/Twixl Publisher/' + myTemplateFolderName + '/' + myTemplateName + '.idml');
                    var myTemplateUserPath1 = new File(Folder.myDocuments + '/Twixl Publisher/' + myTemplateFolderName + '/' + myTemplateName + '.idml');
                    var myTemplateRootPath2 = new File(Folder.appData + '/Twixl Publisher/' + myTemplateFolderName + '/' + myTemplateName + '.indt');
                    var myTemplateUserPath2 = new File(Folder.myDocuments + '/Twixl Publisher/' + myTemplateFolderName + '/' + myTemplateName + '.indt');
                }
                if (myTemplateUserPath1.exists) {
                    myTemplatePath = myTemplateUserPath1;
                } else if (myTemplateUserPath2.exists) {
                    myTemplatePath = myTemplateUserPath2;
                } else if (myTemplateRootPath1.exists) {
                    myTemplatePath = myTemplateRootPath1;
                } else if (myTemplateRootPath2.exists) {
                    myTemplatePath = myTemplateRootPath2;
                } else {
                    TMLogger.error('Failed to find template: ' + myTemplateName);
                    TMDialogs.error('Failed to find template: ' + myTemplateName);
                    TMProgressBar.close();
                    return;
                }
                TMLogger.info('Using template: ' + myTemplatePath.fsName);
                var myDocument = app.open(myTemplatePath, true);
                myDocument.metadataPreferences.documentTitle = myArticleTitle;
                var myMetaData = {
                    title: myArticleTitle,
                    tagline: myArticleTagLine,
                    author: myArticleAuthor,
                    articleURL: myArticleUrl,
                    showInScrubber: myShowInScrubber,
                    backgroundMusicPlaylist: myPlaylist,
                }
                myDocument.label = myMetaData.toSource();
                myDocument.save(myArticleLocation);
                myDocument.sections.firstItem().pageNumberStart = 1;
                var myHtmlResources = new Folder(myDocument.filePath.fsName + "/WebResources");
                if (!myHtmlResources.exists) {
                    TMLogger.info('Creating: ' + myHtmlResources);
                    myHtmlResources.create();
                }
                try {
                    if (myDocument.filePath.readonly) {
                        TMLogger.info('Failed to save the article: file is read-only');
                    } else {
                        myDocument.save();
                    }
                } catch (myException) {
                    TMLogger.error('Failed to save the article: ' + myException + ' (line ' + myException.line + ')');
                }
                myDocument.activeLayer = 'Base Layer'; // myDocument.layers.lastItem();
            }
        } catch (myException) {
            TMLogger.exception('TMBridge.tmNewSingleArticle', myException + ' (tm_bridge:479)');
        }
        TMProgressBar.close();
    },
    tmCurrentArticleName: function () {
        try {
            return app.activeDocument.tmArticleName();
        } catch (myException) {
            TMLogger.exception('TMBridge.tmCurrentArticleName', myException + ' (tm_bridge:490)');
            return undefined;
        }
    },
    tmPreview: function (args) {
        try {
            var myArticleName = args[0];
            var previewOnDevice = args[1];
            var previewDeviceType = args[2]; // undefined when starting from manual ip address
            var previewDevice = args[3];
            var specifier = args[4];
            try {
                var myOptions = app.activeBook.tmProperties();
            } catch (e) {
                var myOptions = {};
            }
            if (myArticleName) {
                myOptions['article'] = myArticleName;
            }
            if (previewDeviceType) {
                myOptions['previewDeviceType'] = previewDeviceType.replace(/ipad_retina/, 'ipad-retina');
            }
            if (previewOnDevice) {
                myOptions['closeProgress'] = false;
                TMExporter.exportDeviceType = previewDeviceType;
            }
            if (myArticleName) {
                if (!myOptions['name']) {
                    myOptions['name'] = TMUtilities.decodeURI(new File(myArticleName).name);
                }
                TMLogger.info('Previewing single article: ' + myArticleName);
                var myExportName = TMUtilities.decodeURI(new File(myArticleName).name).replace(/\//, "").replace(/\\/, "");
            } else {
                TMLogger.info('Previewing publication: ' + myOptions['name']);
                var myExportName = app.activeBook.name.replace(/\//, "").replace(/\\/, "");
            }
            if (myExportName == "") {
                myExportName = "Untitled";
            }
            myExportName += kEXP_EXTENSION_PUBLICATION;
            var myExportFolder = new Folder(Folder.temp.fsName + '/' + kTMP_PREVIEW_PREFIX + TMUtilities.uuid());
            if (!myExportFolder.exists) {
                myExportFolder.create();
            }
            var myExportFile = new File(myExportFolder.fsName + '/' + myExportName);
            if (myArticleName) {
                var myResult = TMExporter.exportArticle(myArticleName, myExportFile.fsName, myOptions);
            } else {
                var myResult = TMExporter.exportPublication(app.activeBook, myExportFile.fsName, myOptions);
            }
            if (myResult) {
                if (previewOnDevice) {
                    TMLogger.info('Previewing on device: ' + previewDevice);
                    TwixlPublisherPluginAPI.previewPublicationOnDevice(myExportFile.fsName, previewDevice);
                } else {
                    TMLogger.info('Previewing on my mac: ' + myExportFile.fsName);
                    TMFiles.writeUTF8DataToFile(myExportFile.fsName + '/preview', specifier);
                    myExportFile.execute();
                    TMLogger.info('Preview finished without problems');
                }
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMBridge.tmPreview', myException);
        }
    },
    tmShare: function (args) {
        try {
            var shareFrom = args[0];
            var shareTo = args[1];
            var shareMessage = args[2];
            var articlePath = args[3];
            if (!shareFrom || shareFrom == '') {
                TMDialogs.error('You need to fill in the from address');
                return;
            }
            if (!shareTo || shareTo == '') {
                TMDialogs.error('You need to fill in the to address');
                return;
            }
            try {
                var myOptions = app.activeBook.tmProperties();
            } catch (e) {
                var myOptions = {};
            }
            if (articlePath) {
                var articleFile = new File(articlePath);
                myOptions['name'] = TMFiles.getBaseName(TMUtilities.decodeURI(articleFile.name), true);
                myOptions['article'] = articlePath;
                TMLogger.info('Sharing article: ' + myOptions['article']);
            } else {
                TMLogger.info('Sharing publication: ' + myOptions['name']);
            }
            var myExportName = TMFiles.filterFileName(myOptions['name']);
            if (myExportName == "") {
                myExportName = "Untitled";
            }
            myExportName += kEXP_EXTENSION_PUBLICATION;
            var myExportFolder = new Folder(Folder.temp.fsName + '/' + kTMP_SHARE_PREFIX + TMUtilities.uuid());
            if (!myExportFolder.exists) {
                myExportFolder.create();
            }
            var myExportFile = new File(myExportFolder.fsName + '/' + myExportName);
            try {
                if (articlePath) {
                    var myResult = TMExporter.exportArticle(articlePath, myExportFile.fsName, myOptions);
                } else {
                    var myResult = TMExporter.exportPublication(app.activeBook, myExportFile.fsName, myOptions);
                }
                if (myResult) {
                    params = {};
                    params['action'] = 'TMShareAction';
                    params['shareFrom'] = shareFrom;
                    params['shareTo'] = shareTo;
                    params['shareMessage'] = shareMessage;
                    params['fileName'] = TMUtilities.decodeURI(myExportFile.name);
                    TMTaskQueue.addTask(myExportFile.fsName, params);
                    TMDialogs.alert('Your file has been submitted for sharing.\n\nExpect a confirmation email in your inbox shortly.', 'Hooray!');
                }
            } catch (myExceptionExport) {
                TMFiles.deleteFolderAndContentsAsync(myExportFolder.fsName);
                TMLogger.exception('TMBridge.tmShare', myExceptionExport + ' (tm_bridge:643)');
            }
        } catch (myException) {
            TMLogger.exception('TMBridge.tmShare', myException + ' (tm_bridge:648)');
        }
    },
    tmExportLocation: function () {
        if (!gLastFolder) {
            gLastFolder = Folder.desktop;
        }
        var myFolderLocation = new Folder(gLastFolder).selectDlg(
            "Select the location for the export"
        );
        if (!myFolderLocation) {
            return undefined;
        }
        gLastFolder = myFolderLocation.fsName;
        var myOptions = app.activeBook.tmProperties();
        var myExportName = TMFiles.filterFileName(app.activeBook.name.replace('.indb', ''));
        if (myExportName == "") {
            myExportName = "Untitled";
        }
        myExportName += kEXP_EXTENSION_PUBLICATION;
        var myExportLocation = myFolderLocation.fsName + "/" + myExportName;
        TMLogger.info('Exporting to: ' + myExportLocation);
        return myExportLocation;
    },
    tmExportArticlesLocation: function () {
        if (!gLastFolder) {
            gLastFolder = Folder.desktop;
        }
        var myFolderLocation = new Folder(gLastFolder).selectDlg(
            "Select the location for the export"
        );
        if (!myFolderLocation) {
            return undefined;
        }
        gLastFolder = myFolderLocation.fsName;
        var myOptions = app.activeBook.tmProperties();
        var myExportName = TMFiles.filterFileName(app.activeBook.name.replace('.indb', ''));
        if (myExportName == "") {
            myExportName = "Untitled";
        }
        myExportName += '_article_exports';
        var myExportLocation = myFolderLocation.fsName + "/" + myExportName;
        TMLogger.info('Exporting to: ' + myExportLocation);
        return myExportLocation;
    },
    tmExportArticleLocation: function (myArticlePath) {
        var myFolderLocation = new Folder(Folder.desktop).selectDlg(
            "Select the location for the export"
        );
        if (!myFolderLocation) {
            return undefined;
        }
        var myArticleFile = new File(myArticlePath);
        var myExportName = TMFiles.getBaseName(TMFiles.filterFileName(TMUtilities.decodeURI(myArticleFile.name)));
        var maxLength = (32 - kEXP_EXTENSION_ARTICLE.length);
        if (myExportName.length > maxLength) {
            myExportName = myExportName.substr(0, maxLength);
        }
        myExportName += kEXP_EXTENSION_ARTICLE;
        var myExportLocation = myFolderLocation.fsName + "/" + myExportName;
        TMLogger.info('Exporting to: ' + myExportLocation);
        return myExportLocation;
    },
    tmExport: function (args) {
        var exportPath = args[0];
        try {
            var myOptions = app.activeBook.tmProperties();
            var myResult = TMExporter.exportPublication(app.activeBook, exportPath, myOptions);
            if (myResult) {
                return exportPath;
            }
        } catch (myException) {
            TMProgressBar.close();
            TMLogger.exception('TMBridge.tmExport', myException + ' (tm_bridge:772)');
            TMDialogs.error(myException);
            return undefined;
        }
    },
    tmExportArticle: function (args) {
        var articlePath = args[0];
        var exportPath = args[1];
        try {
            var myOptions = {
                'alternateLayouts': true,
                'horizontalOnly': false,
                'name': TMFiles.getBaseName(articlePath),
                'showStatusBar': false
            };
            try {
                var myResult = TMExporter.exportArticle(articlePath, exportPath, myOptions);
                if (myResult) {
                    return exportPath;
                }
            } catch (myExceptionExport) {
                TMProgressBar.close();
                TMLogger.exception('TMBridge.tmExportArticle', myExceptionExport + ' (tm_bridge:790)');
                TMDialogs.error(myExceptionExport);
                return undefined;
            }
        } catch (myException) {
            TMProgressBar.close();
            TMLogger.exception('TMBridge.tmExportArticle', myException + ' (tm_bridge:796)');
            TMDialogs.error(myException);
            return undefined;
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMCache = {
    mediaLinks: {},
    pageItemToBeRemoved: {},
    imageSizes: {},
    reset: function () {
        try {
            this.pageItemToBeRemoved = {};
            this.mediaLinks = {};
            this.imageSizes = {};
        } catch (myException) {
            TMLogger.exception("TMCache.reset", myException + ' (tm_cache:18)');
        }
    },
    getImageSize: function (imagePath) {
        return TMHelper.call('image/size', {
            path: imagePath
        }).size;
    },
    isMediaLinkCached: function (myMediaPath) {
        try {
            return this.mediaLinks[myMediaPath] != undefined;
        } catch (myException) {
            TMLogger.exception("TMCache.isMediaLinkCached", myException + ' (tm_cache:30)');
            return false;
        }
    },
    getCachedMediaLink: function (myMediaPath) {
        try {
            return this.mediaLinks[myMediaPath];
        } catch (myException) {
            TMLogger.exception("TMCache.isMediaLinkCached", myException + ' (tm_cache:39)');
        }
    },
    cacheMediaLink: function (myMediaPath, myMediaDstPath) {
        try {
            this.mediaLinks[myMediaPath] = myMediaDstPath;
        } catch (myException) {
            TMLogger.exception("TMCache.cacheMediaLink", myException + ' (tm_cache:47)');
        }
    },
    copyCachedImages: function (document, sectionDimensions, folder, type, suffix) {
        try {
            if (TMPreferences.isDebug()) {
                return false;
            }
            var maxSize = parseInt(TMPreferences.readObject("exportCacheMaxSize", 5)) * 1000 * 1000 * 1000;
            if (maxSize == 0) {
                return false;
            }
            var checkSum = document.tmChecksum();
            if (!checkSum) {
                return false;
            }
            var myCacheFolder = this.cacheFolder().fsName;
            myCacheFolder = new Folder(myCacheFolder + '/' + checkSum + '/' + sectionDimensions + '/' + type + suffix + '/');
            if (myCacheFolder.exists) {
                var myFiles = new Folder(myCacheFolder).getFiles("*.*");
                for (var i in myFiles) {
                    var myFile = myFiles[i];
                    if (myFile instanceof Folder) {
                        continue;
                    }
                    var myFileDst = new File(folder + "/" + myFile.name);
                    TMLogger.info("    Cached: " + sectionDimensions + "/" + document.tmArticleName() + "/" + myFile.name);
                    myFile.copy(myFileDst);
                }
                return true;
            }
            return false;
        } catch (myException) {
            TMLogger.exception("TMCache.copyCachedImages", myException + ' (tm_cache:95)');
            return false;
        }
    },
    copyCachedAsset: function (document, sectionDimensions, filePath) {
        try {
            if (TMPreferences.isDebug()) {
                return false;
            }
            var maxSize = parseInt(TMPreferences.readObject("exportCacheMaxSize", 5)) * 1000 * 1000 * 1000;
            if (maxSize == 0) {
                return false;
            }
            var checkSum = document.tmChecksum();
            if (!checkSum) {
                return false;
            }
            var myFileDst = new File(filePath);
            var myCacheFile = new File(this.cacheFolder().fsName + '/' + checkSum + '/' + sectionDimensions + '/assets/' + myFileDst.name);
            if (myCacheFile.exists) {
                TMLogger.info("    Cached: " + sectionDimensions + "/" + document.tmArticleName() + "/" + myFileDst.name);
                myCacheFile.copy(myFileDst);
                return true;
            }
            return false;
        } catch (myException) {
            TMLogger.exception("TMCache.copyCachedAsset", myException + ' (tm_cache:134)');
            return false;
        }
    },
    cacheImages: function (document, sectionDimensions, folder, type, prefix, suffix) {
        try {
            var checkSum = document.tmChecksum();
            if (!checkSum) {
                return false;
            }
            var myCacheFolder = this.cacheFolder();
            var myFiles = new Folder(folder).getFiles(prefix + "*" + suffix);
            for (var i in myFiles) {
                var myFile = myFiles[i];
                if (myFile instanceof Folder) {
                    continue;
                }
                var myCacheDestination = new File(myCacheFolder.fsName + '/' + checkSum + '/' + sectionDimensions + '/' + type + '/' + myFile.name);
                if (suffix.tmStartsWith("@2x")) {
                    myCacheDestination = new File(myCacheFolder.fsName + '/' + checkSum + '/' + sectionDimensions + '/' + type + '@2x/' + myFile.name);
                }
                if (suffix.tmStartsWith("@3x")) {
                    myCacheDestination = new File(myCacheFolder.fsName + '/' + checkSum + '/' + sectionDimensions + '/' + type + '@3x/' + myFile.name);
                }
                if (!myCacheDestination.parent.exists) {
                    myCacheDestination.parent.create();
                }
                TMLogger.info("    Exported: " + sectionDimensions + "/" + document.tmArticleName() + "/" + myFile.name);
                myFile.copy(myCacheDestination);
            }
        } catch (myException) {
            TMLogger.exception("TMCache.cacheImages", myException + ' (tm_cache:176)');
        }
    },
    cacheAsset: function (document, sectionDimensions, filePath) {
        try {
            var checkSum = document.tmChecksum();
            if (!checkSum) {
                return false;
            }
            var myCacheFolder = this.cacheFolder();
            var myCacheDestination = new File(myCacheFolder.fsName + '/' + checkSum + '/' + sectionDimensions + '/assets/' + new File(filePath).name);
            var myFile = new File(filePath);
            if (!myCacheDestination.parent.exists) {
                myCacheDestination.parent.create();
            }
            TMLogger.info("    Exported: " + sectionDimensions + "/" + document.tmArticleName() + "/" + new File(filePath).name);
            myFile.copy(myCacheDestination);
        } catch (myException) {
            TMLogger.exception("TMCache.cacheAsset", myException + ' (tm_cache:201)');
        }
    },
    cacheFolder: function () {
        try {
            var myCacheFolder = new Folder("~/Library/Caches/com.twixlmedia.publisher.indesign/" + TMVersion.build);
            if (File.fs == "Windows") {
                myCacheFolder = new Folder(Folder.myDocuments + "/Twixl Publisher/Plugin Cache/" + TMVersion.build);
            }
            if (!myCacheFolder.exists) {
                myCacheFolder.create();
            }
            return myCacheFolder;
        } catch (myException) {
            TMLogger.exception("TMCache.cacheAsset", myException + ' (tm_cache:220)');
            return undefined;
        }
    },
    clearCache: function () {
        try {
            TMTaskQueue.addTask(
                undefined, {
                    'action': 'TMClearExportCache',
                    'path': this.cacheFolder().parent.fsName,
                    'path_old': Folder.myDocuments.fsName + "/Twixl Publisher Plugin/Cache/",
                }
            );
        } catch (myException) {
            TMLogger.exception("TMCache.clearCache", myException + ' (tm_cache:236)');
        }
    },
    totalSize: function () {
        var cacheFolder = this.cacheFolder();
        return TMFiles.getFolderSize(cacheFolder.fsName);
    },
    cleanup: function () {
        try {
            TMTaskQueue.addTask(
                undefined, {
                    'action': 'TMCleanup',
                    'path': this.cacheFolder().parent.fsName,
                    'path_old': Folder.myDocuments.fsName + "/Twixl Publisher Plugin/Cache/",
                    'path_tmp': Folder.temp.fsName,
                    'build': TMVersion.build,
                    'maxSize': "" + parseInt(TMPreferences.readObject("exportCacheMaxSize", 5)),
                }
            );
        } catch (myException) {
            TMLogger.exception("TMCache.cleanup", myException + ' (tm_cache:259)');
            return undefined;
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMChecks = {
    hasOpenBook: function () {
        try {
            var book = app.activeBook;
            return true;
        } catch (e) {
            TMDialogs.error("No books are open.");
            return false;
        }
    },
    hasOpenDocument: function () {
        try {
            var book = app.activeDocument;
            return true;
        } catch (e) {
            TMDialogs.error("No documents are open.");
            return false;
        }
    },
    isDocumentSaved: function () {
        if (app.documents.length == 0 || app.activeDocument == undefined) {
            return true;
        }
        try {
            if (app.activeDocument.visible && app.activeDocument.modified) {
                app.activeDocument.save();
            }
        } catch (e) {
            TMLogger.error('Failed to automatically save: ' + app.activeDocument.name);
        }
        if (!app.activeDocument.saved || app.activeDocument.modified) {
            var message = 'The document "' + app.activeDocument.name + '" is not saved.';
            TMDialogs.error(message + '\nPlease save the document before previewing or exporting.');
            return false;
        }
        return true;
    },
    allDocumentsAreSaved: function () {
        var unsavedDocs = [];
        for (var i = 0; i < app.documents.count(); i++) {
            var doc = app.documents.item(i);
            try {
                if (doc.visible && doc.modified) {
                    doc.save();
                }
            } catch (e) {
                TMLogger.error('Failed to automatically save: ' + doc.name);
            }
            if (doc.visible && (!doc.saved || doc.modified)) {
                unsavedDocs.push(doc.name);
            }
        }
        if (unsavedDocs.length > 0) {
            var docNames = '';
            if (unsavedDocs.length == 1) {
                docNames = 'The document "' + unsavedDocs[0] + '" is not saved.';
            } else {
                docNames = 'The documents "' + unsavedDocs.join('", "') + '" are not saved.';
            }
            TMDialogs.error(docNames + '\nPlease save the all documents before previewing or exporting.');
            return false;
        }
        return true;
    },
    bookHasDocuments: function () {
        if (TMPreferences.isRunningLegacyMode() == false) {
            return true;
        }
        if (app.activeBook.tmArticleCount() == 0) {
            TMDialogs.error("The current publication does not contain any articles.");
            return false;
        }
        return true;
    },
    bookUsesAlternateLayouts: function () {
        if (!app.activeBook.usesAlternateLayouts()) {
            TMDialogs.error("The current publication does not use alternate layouts.");
            return false;
        }
        return true;
    },
    isHelperRunning: function () {
        if (!TMHelper.isRunning()) {
            TMDialogs.error("The Twixl Publisher Helper process is not running. Please reinstall the software.\n\nMake sure you open port 53653 on your firewall if needed.\n\nIf the problem persists, please contact support.");
            return false;
        }
        return true;
    }
};

function ColorPicker() { }
ColorPicker.prototype.run = function () {
    var kFontSize = 9;
    var kSwatchBorderWidth = 2;

    function RGBPickerComponent(componentIndex) {
        this.uiResource =
            "group {                                                    \
spacing: 2,                                         \
alignChildren:['right','center'],                   \
swatch: Panel {                                     \
alignment:['left','center'],                    \
properties:{ borderStyle:'sunken' },        \
preferredSize:[18,18]                           \
},                                                          \
label: StaticText { },                              \
slColor: Slider {                                   \
minvalue:0, maxvalue:100                    \
},                                                     \
etColor: EditText {                                 \
characters:3, justify:'right'                   \
},                                                          \
percent: StaticText { text:'%' }            \
}";
        this.initRGBPickerComponent(componentIndex);
    }
    RGBPickerComponent.prototype.initRGBPickerComponent = function (componentIndex) {
        this.rgbVal = [0, 0, 0];
        this.componentIndex = componentIndex;
    }
    RGBPickerComponent.prototype.initialize = function (pickerObj, colorLabel, initialValue) {
        this.pickerObject = pickerObj;
        this.ui = pickerObj.getContainer().add(this.uiResource);
        this.ui.rgbComponentObj = this;
        with (this.ui) {
            label.graphics.font = pickerObj.getUIFont();
            label.text = colorLabel;
            slColor.value = initialValue * slColor.maxvalue;
            slColor.onChanging = slColor.onChange = function () {
                this.parent.etColor.text = this.value;
                this.parent.rgbComponentObj.updateSwatch();
            }
            etColor.graphics.font = pickerObj.getUIFont();
            etColor.onChange = function () {
                var slider = this.parent.slColor;
                var val = Number(this.text);
                if (val > slider.maxvalue)
                    val = slider.maxvalue;
                else if (val < slider.minvalue)
                    val = slider.minvalue;
                slider.value = val;
                this.text = val;
                this.parent.rgbComponentObj.updateSwatch();
            }
        }
        this.setComponentValue(initialValue);
    }
    RGBPickerComponent.prototype.getComponentValue = function () {
        return this.rgbVal[this.componentIndex];
    }
    RGBPickerComponent.prototype.setComponentValue = function (colorVal) {
        this.ui.slColor.value = colorVal * this.ui.slColor.maxvalue;
        this.ui.slColor.notify("onChange");
    }
    RGBPickerComponent.prototype.updateSwatch = function () {
        this.rgbVal[this.componentIndex] = this.ui.slColor.value / this.ui.slColor.maxvalue;
        var uiGfx = this.ui.graphics;
        this.ui.swatch.graphics.backgroundColor = uiGfx.newBrush(uiGfx.BrushType.SOLID_COLOR, this.rgbVal);
        this.ui.swatch.graphics.disabledBackgroundColor = this.ui.swatch.graphics.backgroundColor;
        this.pickerObject.updateRGBSwatch();
    }

    function RGBColorPicker(container, initialRGB, fontSize) {
        this.initRGBColorPicker(container, initialRGB, fontSize);
    }
    RGBColorPicker.prototype.initRGBColorPicker = function (container, initialRGB, fontSize) {
        this.container = container;
        this.rgbValue = initialRGB;
        this.uiFont = ScriptUI.newFont("palette", fontSize);
        this.updatesEnabled = false;
        container.orientation = 'column';
        container.alignChildren = ['fill', 'top'];
        container.spacing = 2;
        this.redPicker = new RGBPickerComponent(0);
        this.greenPicker = new RGBPickerComponent(1);
        this.bluePicker = new RGBPickerComponent(2);
        this.enableUpdates(false);
        this.redPicker.initialize(this, "R:", initialRGB[0]);
        this.greenPicker.initialize(this, "G:", initialRGB[1]);
        this.bluePicker.initialize(this, "B:", 2, initialRGB[2]);
        this.enableUpdates(true);
        this.updateRGBSwatch();
    }
    RGBColorPicker.prototype.getContainer = function () {
        return this.container;
    }
    RGBColorPicker.prototype.getUIFont = function () {
        return this.uiFont;
    }
    RGBColorPicker.prototype.getRGBColor = function () {
        return this.rgbValue;
    }
    RGBColorPicker.prototype.enableUpdates = function (enable) {
        this.updatesEnabled = enable;
    }
    RGBColorPicker.prototype.updateRGBSwatch = function () {
        if (this.updatesEnabled) {
            this.rgbValue[0] = this.redPicker.getComponentValue();
            this.rgbValue[1] = this.greenPicker.getComponentValue();
            this.rgbValue[2] = this.bluePicker.getComponentValue();
            var swatchGfx = this.container.window.rgbSwatch.graphics;
            swatchGfx.backgroundColor =
                swatchGfx.newBrush(swatchGfx.BrushType.SOLID_COLOR, this.rgbValue);
            swatchGfx.disabledBackgroundColor = swatchGfx.backgroundColor;
            this.container.window.rgbSwatch.btn.bgPen =
                swatchGfx.newPen(swatchGfx.PenType.SOLID_COLOR, this.rgbValue, kSwatchBorderWidth);
        }
    }
    try {
        var colorPickerRes = "dialog {  alignChildren: 'fill', text: '', \
main: Group { orientation: 'row', \
text: 'Color Picker', \
orientation:'row', \
rgbSwatch: Group { \
btn: Panel { \
preferredSize:[40,40], \
properties:{ borderStyle:'sunken' }, \
} \
}, \
rgbPicker:  Group {  }, \
}, \
buttons: Group { orientation: 'row', alignment: 'right', \
cancelBtn:    Button { text:'Cancel', properties: {name:'cancel'} }, \
okBtn:        Button { text:'OK',     properties: {name:'ok'} }, \
} \
}";
        var colorPickerWin = new Window(colorPickerRes, 'Select Color', undefined, {
            resizable: false,
            closeButton: false
        });
        colorPickerWin.show();
    } catch (e) {
        TMStackTrace.addToStack("TMColorPicker.run", myException);
    }

    function initializeDrawingState(swatchBtn) {
        var gfx = swatchBtn.graphics;
        var btnW = swatchBtn.size.width;
        var btnH = swatchBtn.size.height;
        var halfBorderW = kSwatchBorderWidth / 2;
        gfx.newPath();
        gfx.moveTo(halfBorderW, btnH - halfBorderW);
        gfx.lineTo(halfBorderW, halfBorderW);
        gfx.lineTo(btnW - halfBorderW, halfBorderW);
        swatchBtn.tlBorderPath = gfx.currentPath;
        gfx.newPath();
        gfx.moveTo(halfBorderW, btnH - halfBorderW);
        gfx.lineTo(btnW - halfBorderW, btnH - halfBorderW);
        gfx.lineTo(btnW - halfBorderW, halfBorderW);
        swatchBtn.brBorderPath = gfx.currentPath;
        swatchBtn.highlightPen = gfx.newPen(gfx.PenType.SOLID_COLOR, [1, 1, 1, .4], kSwatchBorderWidth);
        swatchBtn.shadowPen = gfx.newPen(gfx.PenType.SOLID_COLOR, [.25, .25, .25, .4], kSwatchBorderWidth);
    }

    function drawRGBSwatch(drawingStateObj) {
        var gfx = this.graphics;
        try {
            gfx.strokePath(this.bgPen, this.tlBorderPath);
            gfx.strokePath(this.bgPen, this.brBorderPath);
            gfx.strokePath(this.shadowPen, this.tlBorderPath);
            gfx.strokePath(this.highlightPen, this.brBorderPath);
        } catch (e) {
            this.onDraw = undefined;
            alert("drawRGBSwatch handler failed.\n" + e);
        }
    }
}
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMColorSettings = {
    cmykPolicy: undefined,
    rgbPolicy: undefined,
    enableColorManagement: undefined,
    mismatchAskWhenOpening: undefined,
    missingAskWhenOpening: undefined,
    colorSettingsPath: undefined,
    storeColorSettings: function () {
        this.enableColorManagement = undefined;
        this.rgbPolicy = undefined;
        this.cmykPolicy = undefined;
        this.mismatchAskWhenOpening = undefined;
        this.missingAskWhenOpening = undefined;
        this.colorSettingsPath = undefined;
        try {
            this.enableColorManagement = app.colorSettings.enableColorManagement;
        } catch (myException) {
            TMLogger.exception(myException + ' (tm_color_settings:25)');
        }
        try {
            this.cmykPolicy = app.colorSettings.cmykPolicy;
        } catch (myException) {
            TMLogger.exception(myException + ' (tm_color_settings:30)');
        }
        try {
            this.rgbPolicy = app.colorSettings.rgbPolicy;
        } catch (myException) {
            TMLogger.exception(myException + ' (tm_color_settings:35)');
        }
        try {
            this.mismatchAskWhenOpening = app.colorSettings.mismatchAskWhenOpening;
        } catch (myException) {
            TMLogger.exception(myException + ' (tm_color_settings:40)');
        }
        try {
            this.missingAskWhenOpening = app.colorSettings.missingAskWhenOpening;
        } catch (myException) {
            TMLogger.exception(myException + ' (tm_color_settings:45)');
        }
        try {
            this.colorSettingsPath = app.colorSettings.cmsSettingsPath;
        } catch (myException) {
            TMLogger.exception(myException + ' (tm_color_settings:50)');
        }
    },
    configureColorSettings: function () {
        try {
            this.storeColorSettings();
            try {
                this.enableColorManagement = true;
            } catch (myException) { }
            try {
                this.cmykPolicy = ColorSettingsPolicy.PRESERVE_EMBEDDED_PROFILES;
            } catch (myException) { }
            try {
                this.rgbPolicy = ColorSettingsPolicy.PRESERVE_EMBEDDED_PROFILES;
            } catch (myException) { }
            try {
                this.mismatchAskWhenOpening = false;
            } catch (myException) { }
            try {
                this.missingAskWhenOpening = false;
            } catch (myException) { }
        } catch (myException) {
            TMLogger.exception("TMColorSettings.configureColorSettings", myException + ' (tm_color_settings:78)');
        }
    },
    restoreColorSettings: function () {
        try {
            if (this.enableColorManagement) {
                app.colorSettings.enableColorManagement = this.enableColorManagement;
            }
            if (this.cmykPolicy) {
                app.colorSettings.cmykPolicy = this.cmykPolicy;
            }
            if (this.rgbPolicy) {
                app.colorSettings.rgbPolicy = this.rgbPolicy;
            }
            if (this.mismatchAskWhenOpening) {
                app.colorSettings.mismatchAskWhenOpening = this.mismatchAskWhenOpening;
            }
            if (this.missingAskWhenOpening) {
                app.colorSettings.missingAskWhenOpening = this.missingAskWhenOpening;
            }
            if (this.colorSettingsPath) {
                try {
                    app.colorSettings.cmsSettingsPath = this.colorSettingsPath;
                } catch (myException) { }
            }
        } catch (myException) {
            TMLogger.exception("TMColorSettings.restoreColorSettings", myException + ' (tm_color_settings:106)');
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var kAPP_NAME = "Twixl Publisher";
var kPREFLIGHT_PROFILE = "Twixl Publisher";
var kPREFLIGHT_PROFILE_FILE = "Twixl Publisher.idpp";
var kEXP_EXTENSION_PUBLICATION = ".publication";
var kEXP_EXTENSION_ARTICLE = ".article";
var kTMP_PREFIX = 'tp_';
var kTMP_PREVIEW_PREFIX = kTMP_PREFIX + 'preview_';
var kTMP_SHARE_PREFIX = kTMP_PREFIX + 'share_';
var kPORT_VIEWER = 53652;
var kPORT_HELPER = 53653;
var kPRECISION = 10;
var kEXPORT_FORMAT = 'JPG'; // PDF or JPG
var kKEEP_TEMP_PAGES = false;
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
/**
 * TMCrypto core components.
 */
var TMCrypto = TMCrypto || (function (Math, undefined) {
    /**
     * TMCrypto namespace.
     */
    var C = {};
    /**
     * Library namespace.
     */
    var C_lib = C.lib = {};
    /**
     * Base object for prototypal inheritance.
     */
    var Base = C_lib.Base = (function () {
        function F() { }
        return {
            /**
             * Creates a new object that inherits from this object.
             *
             * @param {Object} overrides Properties to copy into the new object.
             *
             * @return {Object} The new object.
             *
             * @static
             *
             * @example
             *
             *     var MyType = TMCrypto.lib.Base.extend({
             *         field: 'value',
             *
             *         method: function () {
             *         }
             *     });
             */
            extend: function (overrides) {
                F.prototype = this;
                var subtype = new F();
                if (overrides) {
                    subtype.mixIn(overrides);
                }
                if (!subtype.hasOwnProperty('init')) {
                    subtype.init = function () {
                        subtype.$super.init.apply(this, arguments);
                    };
                }
                subtype.init.prototype = subtype;
                subtype.$super = this;
                return subtype;
            },
            /**
             * Extends this object and runs the init method.
             * Arguments to create() will be passed to init().
             *
             * @return {Object} The new object.
             *
             * @static
             *
             * @example
             *
             *     var instance = MyType.create();
             */
            create: function () {
                var instance = this.extend();
                instance.init.apply(instance, arguments);
                return instance;
            },
            /**
             * Initializes a newly created object.
             * Override this method to add some logic when your objects are created.
             *
             * @example
             *
             *     var MyType = TMCrypto.lib.Base.extend({
             *         init: function () {
             *             // ...
             *         }
             *     });
             */
            init: function () { },
            /**
             * Copies properties into this object.
             *
             * @param {Object} properties The properties to mix in.
             *
             * @example
             *
             *     MyType.mixIn({
             *         field: 'value'
             *     });
             */
            mixIn: function (properties) {
                for (var propertyName in properties) {
                    if (properties.hasOwnProperty(propertyName)) {
                        this[propertyName] = properties[propertyName];
                    }
                }
                if (properties.hasOwnProperty('toString')) {
                    this.toString = properties.toString;
                }
            },
            /**
             * Creates a copy of this object.
             *
             * @return {Object} The clone.
             *
             * @example
             *
             *     var clone = instance.clone();
             */
            clone: function () {
                return this.init.prototype.extend(this);
            }
        };
    }());
    /**
     * An array of 32-bit words.
     *
     * @property {Array} words The array of 32-bit words.
     * @property {number} sigBytes The number of significant bytes in this word array.
     */
    var WordArray = C_lib.WordArray = Base.extend({
        /**
         * Initializes a newly created word array.
         *
         * @param {Array} words (Optional) An array of 32-bit words.
         * @param {number} sigBytes (Optional) The number of significant bytes in the words.
         *
         * @example
         *
         *     var wordArray = TMCrypto.lib.WordArray.create();
         *     var wordArray = TMCrypto.lib.WordArray.create([0x00010203, 0x04050607]);
         *     var wordArray = TMCrypto.lib.WordArray.create([0x00010203, 0x04050607], 6);
         */
        init: function (words, sigBytes) {
            words = this.words = words || [];
            if (sigBytes != undefined) {
                this.sigBytes = sigBytes;
            } else {
                this.sigBytes = words.length * 4;
            }
        },
        /**
         * Converts this word array to a string.
         *
         * @param {Encoder} encoder (Optional) The encoding strategy to use. Default: TMCrypto.enc.Hex
         *
         * @return {string} The stringified word array.
         *
         * @example
         *
         *     var string = wordArray + '';
         *     var string = wordArray.toString();
         *     var string = wordArray.toString(TMCrypto.enc.Utf8);
         */
        toString: function (encoder) {
            return (encoder || Hex).stringify(this);
        },
        /**
         * Concatenates a word array to this word array.
         *
         * @param {WordArray} wordArray The word array to append.
         *
         * @return {WordArray} This word array.
         *
         * @example
         *
         *     wordArray1.concat(wordArray2);
         */
        concat: function (wordArray) {
            var thisWords = this.words;
            var thatWords = wordArray.words;
            var thisSigBytes = this.sigBytes;
            var thatSigBytes = wordArray.sigBytes;
            this.clamp();
            if (thisSigBytes % 4) {
                for (var i = 0; i < thatSigBytes; i++) {
                    var thatByte = (thatWords[i >>> 2] >>> (24 - (i % 4) * 8)) & 0xff;
                    thisWords[(thisSigBytes + i) >>> 2] |= thatByte << (24 - ((thisSigBytes + i) % 4) * 8);
                }
            } else if (thatWords.length > 0xffff) {
                for (var i = 0; i < thatSigBytes; i += 4) {
                    thisWords[(thisSigBytes + i) >>> 2] = thatWords[i >>> 2];
                }
            } else {
                thisWords.push.apply(thisWords, thatWords);
            }
            this.sigBytes += thatSigBytes;
            return this;
        },
        /**
         * Removes insignificant bits.
         *
         * @example
         *
         *     wordArray.clamp();
         */
        clamp: function () {
            var words = this.words;
            var sigBytes = this.sigBytes;
            words[sigBytes >>> 2] &= 0xffffffff << (32 - (sigBytes % 4) * 8);
            words.length = Math.ceil(sigBytes / 4);
        },
        /**
         * Creates a copy of this word array.
         *
         * @return {WordArray} The clone.
         *
         * @example
         *
         *     var clone = wordArray.clone();
         */
        clone: function () {
            var clone = Base.clone.call(this);
            clone.words = this.words.slice(0);
            return clone;
        },
        /**
         * Creates a word array filled with random bytes.
         *
         * @param {number} nBytes The number of random bytes to generate.
         *
         * @return {WordArray} The random word array.
         *
         * @static
         *
         * @example
         *
         *     var wordArray = TMCrypto.lib.WordArray.random(16);
         */
        random: function (nBytes) {
            var words = [];
            for (var i = 0; i < nBytes; i += 4) {
                words.push((Math.random() * 0x100000000) | 0);
            }
            return new WordArray.init(words, nBytes);
        }
    });
    /**
     * Encoder namespace.
     */
    var C_enc = C.enc = {};
    /**
     * Hex encoding strategy.
     */
    var Hex = C_enc.Hex = {
        /**
         * Converts a word array to a hex string.
         *
         * @param {WordArray} wordArray The word array.
         *
         * @return {string} The hex string.
         *
         * @static
         *
         * @example
         *
         *     var hexString = TMCrypto.enc.Hex.stringify(wordArray);
         */
        stringify: function (wordArray) {
            var words = wordArray.words;
            var sigBytes = wordArray.sigBytes;
            var hexChars = [];
            for (var i = 0; i < sigBytes; i++) {
                var bite = (words[i >>> 2] >>> (24 - (i % 4) * 8)) & 0xff;
                hexChars.push((bite >>> 4).toString(16));
                hexChars.push((bite & 0x0f).toString(16));
            }
            return hexChars.join('');
        },
        /**
         * Converts a hex string to a word array.
         *
         * @param {string} hexStr The hex string.
         *
         * @return {WordArray} The word array.
         *
         * @static
         *
         * @example
         *
         *     var wordArray = TMCrypto.enc.Hex.parse(hexString);
         */
        parse: function (hexStr) {
            var hexStrLength = hexStr.length;
            var words = [];
            for (var i = 0; i < hexStrLength; i += 2) {
                words[i >>> 3] |= parseInt(hexStr.substr(i, 2), 16) << (24 - (i % 8) * 4);
            }
            return new WordArray.init(words, hexStrLength / 2);
        }
    };
    /**
     * Latin1 encoding strategy.
     */
    var Latin1 = C_enc.Latin1 = {
        /**
         * Converts a word array to a Latin1 string.
         *
         * @param {WordArray} wordArray The word array.
         *
         * @return {string} The Latin1 string.
         *
         * @static
         *
         * @example
         *
         *     var latin1String = TMCrypto.enc.Latin1.stringify(wordArray);
         */
        stringify: function (wordArray) {
            var words = wordArray.words;
            var sigBytes = wordArray.sigBytes;
            var latin1Chars = [];
            for (var i = 0; i < sigBytes; i++) {
                var bite = (words[i >>> 2] >>> (24 - (i % 4) * 8)) & 0xff;
                latin1Chars.push(String.fromCharCode(bite));
            }
            return latin1Chars.join('');
        },
        /**
         * Converts a Latin1 string to a word array.
         *
         * @param {string} latin1Str The Latin1 string.
         *
         * @return {WordArray} The word array.
         *
         * @static
         *
         * @example
         *
         *     var wordArray = TMCrypto.enc.Latin1.parse(latin1String);
         */
        parse: function (latin1Str) {
            var latin1StrLength = latin1Str.length;
            var words = [];
            for (var i = 0; i < latin1StrLength; i++) {
                words[i >>> 2] |= (latin1Str.charCodeAt(i) & 0xff) << (24 - (i % 4) * 8);
            }
            return new WordArray.init(words, latin1StrLength);
        }
    };
    /**
     * UTF-8 encoding strategy.
     */
    var Utf8 = C_enc.Utf8 = {
        /**
         * Converts a word array to a UTF-8 string.
         *
         * @param {WordArray} wordArray The word array.
         *
         * @return {string} The UTF-8 string.
         *
         * @static
         *
         * @example
         *
         *     var utf8String = TMCrypto.enc.Utf8.stringify(wordArray);
         */
        stringify: function (wordArray) {
            try {
                return decodeURIComponent(escape(Latin1.stringify(wordArray)));
            } catch (e) {
                throw new Error('Malformed UTF-8 data');
            }
        },
        /**
         * Converts a UTF-8 string to a word array.
         *
         * @param {string} utf8Str The UTF-8 string.
         *
         * @return {WordArray} The word array.
         *
         * @static
         *
         * @example
         *
         *     var wordArray = TMCrypto.enc.Utf8.parse(utf8String);
         */
        parse: function (utf8Str) {
            return Latin1.parse(unescape(encodeURIComponent(utf8Str)));
        }
    };
    /**
     * Abstract buffered block algorithm template.
     *
     * The property blockSize must be implemented in a concrete subtype.
     *
     * @property {number} _minBufferSize The number of blocks that should be kept unprocessed in the buffer. Default: 0
     */
    var BufferedBlockAlgorithm = C_lib.BufferedBlockAlgorithm = Base.extend({
        /**
         * Resets this block algorithm's data buffer to its initial state.
         *
         * @example
         *
         *     bufferedBlockAlgorithm.reset();
         */
        reset: function () {
            this._data = new WordArray.init();
            this._nDataBytes = 0;
        },
        /**
         * Adds new data to this block algorithm's buffer.
         *
         * @param {WordArray|string} data The data to append. Strings are converted to a WordArray using UTF-8.
         *
         * @example
         *
         *     bufferedBlockAlgorithm._append('data');
         *     bufferedBlockAlgorithm._append(wordArray);
         */
        _append: function (data) {
            if (typeof data == 'string') {
                data = Utf8.parse(data);
            }
            this._data.concat(data);
            this._nDataBytes += data.sigBytes;
        },
        /**
         * Processes available data blocks.
         *
         * This method invokes _doProcessBlock(offset), which must be implemented by a concrete subtype.
         *
         * @param {boolean} doFlush Whether all blocks and partial blocks should be processed.
         *
         * @return {WordArray} The processed data.
         *
         * @example
         *
         *     var processedData = bufferedBlockAlgorithm._process();
         *     var processedData = bufferedBlockAlgorithm._process(!!'flush');
         */
        _process: function (doFlush) {
            var data = this._data;
            var dataWords = data.words;
            var dataSigBytes = data.sigBytes;
            var blockSize = this.blockSize;
            var blockSizeBytes = blockSize * 4;
            var nBlocksReady = dataSigBytes / blockSizeBytes;
            if (doFlush) {
                nBlocksReady = Math.ceil(nBlocksReady);
            } else {
                nBlocksReady = Math.max((nBlocksReady | 0) - this._minBufferSize, 0);
            }
            var nWordsReady = nBlocksReady * blockSize;
            var nBytesReady = Math.min(nWordsReady * 4, dataSigBytes);
            if (nWordsReady) {
                for (var offset = 0; offset < nWordsReady; offset += blockSize) {
                    this._doProcessBlock(dataWords, offset);
                }
                var processedWords = dataWords.splice(0, nWordsReady);
                data.sigBytes -= nBytesReady;
            }
            return new WordArray.init(processedWords, nBytesReady);
        },
        /**
         * Creates a copy of this object.
         *
         * @return {Object} The clone.
         *
         * @example
         *
         *     var clone = bufferedBlockAlgorithm.clone();
         */
        clone: function () {
            var clone = Base.clone.call(this);
            clone._data = this._data.clone();
            return clone;
        },
        _minBufferSize: 0
    });
    /**
     * Abstract hasher template.
     *
     * @property {number} blockSize The number of 32-bit words this hasher operates on. Default: 16 (512 bits)
     */
    var Hasher = C_lib.Hasher = BufferedBlockAlgorithm.extend({
        /**
         * Configuration options.
         */
        cfg: Base.extend(),
        /**
         * Initializes a newly created hasher.
         *
         * @param {Object} cfg (Optional) The configuration options to use for this hash computation.
         *
         * @example
         *
         *     var hasher = TMCrypto.algo.SHA256.create();
         */
        init: function (cfg) {
            this.cfg = this.cfg.extend(cfg);
            this.reset();
        },
        /**
         * Resets this hasher to its initial state.
         *
         * @example
         *
         *     hasher.reset();
         */
        reset: function () {
            BufferedBlockAlgorithm.reset.call(this);
            this._doReset();
        },
        /**
         * Updates this hasher with a message.
         *
         * @param {WordArray|string} messageUpdate The message to append.
         *
         * @return {Hasher} This hasher.
         *
         * @example
         *
         *     hasher.update('message');
         *     hasher.update(wordArray);
         */
        update: function (messageUpdate) {
            this._append(messageUpdate);
            this._process();
            return this;
        },
        /**
         * Finalizes the hash computation.
         * Note that the finalize operation is effectively a destructive, read-once operation.
         *
         * @param {WordArray|string} messageUpdate (Optional) A final message update.
         *
         * @return {WordArray} The hash.
         *
         * @example
         *
         *     var hash = hasher.finalize();
         *     var hash = hasher.finalize('message');
         *     var hash = hasher.finalize(wordArray);
         */
        finalize: function (messageUpdate) {
            if (messageUpdate) {
                this._append(messageUpdate);
            }
            var hash = this._doFinalize();
            return hash;
        },
        blockSize: 512 / 32,
        /**
         * Creates a shortcut function to a hasher's object interface.
         *
         * @param {Hasher} hasher The hasher to create a helper for.
         *
         * @return {Function} The shortcut function.
         *
         * @static
         *
         * @example
         *
         *     var SHA256 = TMCrypto.lib.Hasher._createHelper(TMCrypto.algo.SHA256);
         */
        _createHelper: function (hasher) {
            return function (message, cfg) {
                return new hasher.init(cfg).finalize(message);
            };
        },
        /**
         * Creates a shortcut function to the HMAC's object interface.
         *
         * @param {Hasher} hasher The hasher to use in this HMAC helper.
         *
         * @return {Function} The shortcut function.
         *
         * @static
         *
         * @example
         *
         *     var HmacSHA256 = TMCrypto.lib.Hasher._createHmacHelper(TMCrypto.algo.SHA256);
         */
        _createHmacHelper: function (hasher) {
            return function (message, key) {
                return new C_algo.HMAC.init(hasher, key).finalize(message);
            };
        }
    });
    /**
     * Algorithm namespace.
     */
    var C_algo = C.algo = {};
    return C;
}(Math));
(function () {
    var C = TMCrypto;
    var C_lib = C.lib;
    var WordArray = C_lib.WordArray;
    var Hasher = C_lib.Hasher;
    var C_algo = C.algo;
    var W = [];
    /**
     * SHA-1 hash algorithm.
     */
    var SHA1 = C_algo.SHA1 = Hasher.extend({
        _doReset: function () {
            this._hash = new WordArray.init([
                0x67452301, 0xefcdab89,
                0x98badcfe, 0x10325476,
                0xc3d2e1f0
            ]);
        },
        _doProcessBlock: function (M, offset) {
            var H = this._hash.words;
            var a = H[0];
            var b = H[1];
            var c = H[2];
            var d = H[3];
            var e = H[4];
            for (var i = 0; i < 80; i++) {
                if (i < 16) {
                    W[i] = M[offset + i] | 0;
                } else {
                    var n = W[i - 3] ^ W[i - 8] ^ W[i - 14] ^ W[i - 16];
                    W[i] = (n << 1) | (n >>> 31);
                }
                var t = ((a << 5) | (a >>> 27)) + e + W[i];
                if (i < 20) {
                    t += ((b & c) | (~b & d)) + 0x5a827999;
                } else if (i < 40) {
                    t += (b ^ c ^ d) + 0x6ed9eba1;
                } else if (i < 60) {
                    t += ((b & c) | (b & d) | (c & d)) - 0x70e44324;
                } else /* if (i < 80) */ {
                    t += (b ^ c ^ d) - 0x359d3e2a;
                }
                e = d;
                d = c;
                c = (b << 30) | (b >>> 2);
                b = a;
                a = t;
            }
            H[0] = (H[0] + a) | 0;
            H[1] = (H[1] + b) | 0;
            H[2] = (H[2] + c) | 0;
            H[3] = (H[3] + d) | 0;
            H[4] = (H[4] + e) | 0;
        },
        _doFinalize: function () {
            var data = this._data;
            var dataWords = data.words;
            var nBitsTotal = this._nDataBytes * 8;
            var nBitsLeft = data.sigBytes * 8;
            dataWords[nBitsLeft >>> 5] |= 0x80 << (24 - nBitsLeft % 32);
            dataWords[(((nBitsLeft + 64) >>> 9) << 4) + 14] = Math.floor(nBitsTotal / 0x100000000);
            dataWords[(((nBitsLeft + 64) >>> 9) << 4) + 15] = nBitsTotal;
            data.sigBytes = dataWords.length * 4;
            this._process();
            return this._hash;
        },
        clone: function () {
            var clone = Hasher.clone.call(this);
            clone._hash = this._hash.clone();
            return clone;
        }
    });
    /**
     * Shortcut function to the hasher's object interface.
     *
     * @param {WordArray|string} message The message to hash.
     *
     * @return {WordArray} The hash.
     *
     * @static
     *
     * @example
     *
     *     var hash = TMCrypto.SHA1('message');
     *     var hash = TMCrypto.SHA1(wordArray);
     */
    C.SHA1 = Hasher._createHelper(SHA1);
    /**
     * Shortcut function to the HMAC's object interface.
     *
     * @param {WordArray|string} message The message to hash.
     * @param {WordArray|string} key The secret key.
     *
     * @return {WordArray} The HMAC.
     *
     * @static
     *
     * @example
     *
     *     var hmac = TMCrypto.HmacSHA1(message, key);
     */
    C.HmacSHA1 = Hasher._createHmacHelper(SHA1);
    var MD5 = function (string) {
        function RotateLeft(lValue, iShiftBits) {
            return (lValue << iShiftBits) | (lValue >>> (32 - iShiftBits));
        }

        function AddUnsigned(lX, lY) {
            var lX4, lY4, lX8, lY8, lResult;
            lX8 = (lX & 0x80000000);
            lY8 = (lY & 0x80000000);
            lX4 = (lX & 0x40000000);
            lY4 = (lY & 0x40000000);
            lResult = (lX & 0x3FFFFFFF) + (lY & 0x3FFFFFFF);
            if (lX4 & lY4) {
                return (lResult ^ 0x80000000 ^ lX8 ^ lY8);
            }
            if (lX4 | lY4) {
                if (lResult & 0x40000000) {
                    return (lResult ^ 0xC0000000 ^ lX8 ^ lY8);
                } else {
                    return (lResult ^ 0x40000000 ^ lX8 ^ lY8);
                }
            } else {
                return (lResult ^ lX8 ^ lY8);
            }
        }

        function F(x, y, z) {
            return (x & y) | ((~x) & z);
        }

        function G(x, y, z) {
            return (x & z) | (y & (~z));
        }

        function H(x, y, z) {
            return (x ^ y ^ z);
        }

        function I(x, y, z) {
            return (y ^ (x | (~z)));
        }

        function FF(a, b, c, d, x, s, ac) {
            a = AddUnsigned(a, AddUnsigned(AddUnsigned(F(b, c, d), x), ac));
            return AddUnsigned(RotateLeft(a, s), b);
        };

        function GG(a, b, c, d, x, s, ac) {
            a = AddUnsigned(a, AddUnsigned(AddUnsigned(G(b, c, d), x), ac));
            return AddUnsigned(RotateLeft(a, s), b);
        };

        function HH(a, b, c, d, x, s, ac) {
            a = AddUnsigned(a, AddUnsigned(AddUnsigned(H(b, c, d), x), ac));
            return AddUnsigned(RotateLeft(a, s), b);
        };

        function II(a, b, c, d, x, s, ac) {
            a = AddUnsigned(a, AddUnsigned(AddUnsigned(I(b, c, d), x), ac));
            return AddUnsigned(RotateLeft(a, s), b);
        };

        function ConvertToWordArray(string) {
            var lWordCount;
            var lMessageLength = string.length;
            var lNumberOfWords_temp1 = lMessageLength + 8;
            var lNumberOfWords_temp2 = (lNumberOfWords_temp1 - (lNumberOfWords_temp1 % 64)) / 64;
            var lNumberOfWords = (lNumberOfWords_temp2 + 1) * 16;
            var lWordArray = Array(lNumberOfWords - 1);
            var lBytePosition = 0;
            var lByteCount = 0;
            while (lByteCount < lMessageLength) {
                lWordCount = (lByteCount - (lByteCount % 4)) / 4;
                lBytePosition = (lByteCount % 4) * 8;
                lWordArray[lWordCount] = (lWordArray[lWordCount] | (string.charCodeAt(lByteCount) << lBytePosition));
                lByteCount++;
            }
            lWordCount = (lByteCount - (lByteCount % 4)) / 4;
            lBytePosition = (lByteCount % 4) * 8;
            lWordArray[lWordCount] = lWordArray[lWordCount] | (0x80 << lBytePosition);
            lWordArray[lNumberOfWords - 2] = lMessageLength << 3;
            lWordArray[lNumberOfWords - 1] = lMessageLength >>> 29;
            return lWordArray;
        };

        function WordToHex(lValue) {
            var WordToHexValue = "",
                WordToHexValue_temp = "",
                lByte, lCount;
            for (lCount = 0; lCount <= 3; lCount++) {
                lByte = (lValue >>> (lCount * 8)) & 255;
                WordToHexValue_temp = "0" + lByte.toString(16);
                WordToHexValue = WordToHexValue + WordToHexValue_temp.substr(WordToHexValue_temp.length - 2, 2);
            }
            return WordToHexValue;
        };

        function Utf8Encode(string) {
            string = string.replace(/\r\n/g, "\n");
            var utftext = "";
            for (var n = 0; n < string.length; n++) {
                var c = string.charCodeAt(n);
                if (c < 128) {
                    utftext += String.fromCharCode(c);
                } else if ((c > 127) && (c < 2048)) {
                    utftext += String.fromCharCode((c >> 6) | 192);
                    utftext += String.fromCharCode((c & 63) | 128);
                } else {
                    utftext += String.fromCharCode((c >> 12) | 224);
                    utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                    utftext += String.fromCharCode((c & 63) | 128);
                }
            }
            return utftext;
        };
        var x = Array();
        var k, AA, BB, CC, DD, a, b, c, d;
        var S11 = 7,
            S12 = 12,
            S13 = 17,
            S14 = 22;
        var S21 = 5,
            S22 = 9,
            S23 = 14,
            S24 = 20;
        var S31 = 4,
            S32 = 11,
            S33 = 16,
            S34 = 23;
        var S41 = 6,
            S42 = 10,
            S43 = 15,
            S44 = 21;
        string = Utf8Encode(string);
        x = ConvertToWordArray(string);
        a = 0x67452301;
        b = 0xEFCDAB89;
        c = 0x98BADCFE;
        d = 0x10325476;
        for (k = 0; k < x.length; k += 16) {
            AA = a;
            BB = b;
            CC = c;
            DD = d;
            a = FF(a, b, c, d, x[k + 0], S11, 0xD76AA478);
            d = FF(d, a, b, c, x[k + 1], S12, 0xE8C7B756);
            c = FF(c, d, a, b, x[k + 2], S13, 0x242070DB);
            b = FF(b, c, d, a, x[k + 3], S14, 0xC1BDCEEE);
            a = FF(a, b, c, d, x[k + 4], S11, 0xF57C0FAF);
            d = FF(d, a, b, c, x[k + 5], S12, 0x4787C62A);
            c = FF(c, d, a, b, x[k + 6], S13, 0xA8304613);
            b = FF(b, c, d, a, x[k + 7], S14, 0xFD469501);
            a = FF(a, b, c, d, x[k + 8], S11, 0x698098D8);
            d = FF(d, a, b, c, x[k + 9], S12, 0x8B44F7AF);
            c = FF(c, d, a, b, x[k + 10], S13, 0xFFFF5BB1);
            b = FF(b, c, d, a, x[k + 11], S14, 0x895CD7BE);
            a = FF(a, b, c, d, x[k + 12], S11, 0x6B901122);
            d = FF(d, a, b, c, x[k + 13], S12, 0xFD987193);
            c = FF(c, d, a, b, x[k + 14], S13, 0xA679438E);
            b = FF(b, c, d, a, x[k + 15], S14, 0x49B40821);
            a = GG(a, b, c, d, x[k + 1], S21, 0xF61E2562);
            d = GG(d, a, b, c, x[k + 6], S22, 0xC040B340);
            c = GG(c, d, a, b, x[k + 11], S23, 0x265E5A51);
            b = GG(b, c, d, a, x[k + 0], S24, 0xE9B6C7AA);
            a = GG(a, b, c, d, x[k + 5], S21, 0xD62F105D);
            d = GG(d, a, b, c, x[k + 10], S22, 0x2441453);
            c = GG(c, d, a, b, x[k + 15], S23, 0xD8A1E681);
            b = GG(b, c, d, a, x[k + 4], S24, 0xE7D3FBC8);
            a = GG(a, b, c, d, x[k + 9], S21, 0x21E1CDE6);
            d = GG(d, a, b, c, x[k + 14], S22, 0xC33707D6);
            c = GG(c, d, a, b, x[k + 3], S23, 0xF4D50D87);
            b = GG(b, c, d, a, x[k + 8], S24, 0x455A14ED);
            a = GG(a, b, c, d, x[k + 13], S21, 0xA9E3E905);
            d = GG(d, a, b, c, x[k + 2], S22, 0xFCEFA3F8);
            c = GG(c, d, a, b, x[k + 7], S23, 0x676F02D9);
            b = GG(b, c, d, a, x[k + 12], S24, 0x8D2A4C8A);
            a = HH(a, b, c, d, x[k + 5], S31, 0xFFFA3942);
            d = HH(d, a, b, c, x[k + 8], S32, 0x8771F681);
            c = HH(c, d, a, b, x[k + 11], S33, 0x6D9D6122);
            b = HH(b, c, d, a, x[k + 14], S34, 0xFDE5380C);
            a = HH(a, b, c, d, x[k + 1], S31, 0xA4BEEA44);
            d = HH(d, a, b, c, x[k + 4], S32, 0x4BDECFA9);
            c = HH(c, d, a, b, x[k + 7], S33, 0xF6BB4B60);
            b = HH(b, c, d, a, x[k + 10], S34, 0xBEBFBC70);
            a = HH(a, b, c, d, x[k + 13], S31, 0x289B7EC6);
            d = HH(d, a, b, c, x[k + 0], S32, 0xEAA127FA);
            c = HH(c, d, a, b, x[k + 3], S33, 0xD4EF3085);
            b = HH(b, c, d, a, x[k + 6], S34, 0x4881D05);
            a = HH(a, b, c, d, x[k + 9], S31, 0xD9D4D039);
            d = HH(d, a, b, c, x[k + 12], S32, 0xE6DB99E5);
            c = HH(c, d, a, b, x[k + 15], S33, 0x1FA27CF8);
            b = HH(b, c, d, a, x[k + 2], S34, 0xC4AC5665);
            a = II(a, b, c, d, x[k + 0], S41, 0xF4292244);
            d = II(d, a, b, c, x[k + 7], S42, 0x432AFF97);
            c = II(c, d, a, b, x[k + 14], S43, 0xAB9423A7);
            b = II(b, c, d, a, x[k + 5], S44, 0xFC93A039);
            a = II(a, b, c, d, x[k + 12], S41, 0x655B59C3);
            d = II(d, a, b, c, x[k + 3], S42, 0x8F0CCC92);
            c = II(c, d, a, b, x[k + 10], S43, 0xFFEFF47D);
            b = II(b, c, d, a, x[k + 1], S44, 0x85845DD1);
            a = II(a, b, c, d, x[k + 8], S41, 0x6FA87E4F);
            d = II(d, a, b, c, x[k + 15], S42, 0xFE2CE6E0);
            c = II(c, d, a, b, x[k + 6], S43, 0xA3014314);
            b = II(b, c, d, a, x[k + 13], S44, 0x4E0811A1);
            a = II(a, b, c, d, x[k + 4], S41, 0xF7537E82);
            d = II(d, a, b, c, x[k + 11], S42, 0xBD3AF235);
            c = II(c, d, a, b, x[k + 2], S43, 0x2AD7D2BB);
            b = II(b, c, d, a, x[k + 9], S44, 0xEB86D391);
            a = AddUnsigned(a, AA);
            b = AddUnsigned(b, BB);
            c = AddUnsigned(c, CC);
            d = AddUnsigned(d, DD);
        }
        var temp = WordToHex(a) + WordToHex(b) + WordToHex(c) + WordToHex(d);
        return temp.toLowerCase();
    }
    C.MD5 = MD5;
}());
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMDialogs = {
    alert: function (message, title, showError) {
        try {
            TMLogger.error(message);
            if (!showError) {
                showError = false;
            }
            Window.alert(message, title, showError);
        } catch (myException) {
            TMStackTrace.addToStack('TMDialogs.alert', myException);
        }
    },
    message: function (message, title) {
        try {
            if (Folder.fs == 'Windows') {
                TMDialogs.alert(message, title, false);
            } else {
                TMDialogs.alert(title + '\n' + message, '', false);
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMDialogs.message', myException);
        }
    },
    error: function (message) {
        try {
            TMLogger.error(message);
            if (Folder.fs == 'Windows') {
                TMDialogs.alert(message, 'An Error Occurred', true);
            } else {
                TMDialogs.alert('An Error Occurred\n' + message, '', true);
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMDialogs.error', myException);
        }
    },
    confirm: function (message, title, noAsDefault) {
        try {
            if (noAsDefault == undefined) {
                noAsDefault = false;
            }
            return Window.confirm(message, noAsDefault, title);
        } catch (myException) {
            TMStackTrace.addToStack('TMDialogs.confirm', myException);
            return false;
        }
    },
    confirm2: function (message, title, noAsDefault) {
        if (TMDialogs.confirm(message, title, noAsDefault)) {
            return 'true';
        } else {
            return 'false';
        }
    },
    showException: function (jsFunctionName, myException) {
        var message = myException.toString() + '\n\n' + TMStackTrace.getStackTrace(myException);
        TMLogger.error(message);
        if (Folder.fs == 'Windows') {
            Window.alert(message, 'An Error Occurred', true);
        } else {
            Window.alert('An Error Occurred\n' + message, '', true);
        }
    },
    _configureExportFormat: function (myDialog, exportFormat, exportDesc) {
        if (exportFormat == undefined) {
            exportFormat = myDialog.exportFormatPanel.exportFormatGroup.exportFormat;
        }
        var exportAsPDF = exportFormat.add('item', 'PDF');
        var exportAsJPG = exportFormat.add('item', 'JPG');
        var defaultValue = TMPreferences.readObject('export_format', 'PDF');
        if (defaultValue == 'JPG') {
            exportAsJPG.selected = true;
        } else {
            exportAsPDF.selected = true;
        }
        if (exportDesc == undefined) {
            exportDesc = myDialog.exportFormatPanel.exportDescGroup.exportFormat;
        }
        exportDesc.minimumSize.width = TMUI.UI_ITEM_WIDTH;
        exportDesc.justify = 'left';
        exportFormat.onChange = function () {
            if (exportFormat.selection.text == 'PDF') {
                exportDesc.text = "Requires Twixl Reader / Viewer 4.2 or newer";
            } else {
                exportDesc.text = "Compatible with all Twixl Reader / Viewer versions";
            }
            kEXPORT_FORMAT = exportFormat.selection.text;
            TMPreferences.saveObject('export_format', kEXPORT_FORMAT);
        };
        exportFormat.onChange();
    },
    selectExportFormat: function () {
        try {
            var myRes = "dialog { alignChildren: 'fill', text: '', \
exportFormatPanel: Panel { orientation: 'column', alignChildren: 'left', text: '', \
exportFormatGroup: Group { orientation: 'row', \
exportFormatLabel: StaticText { text: 'Export As' }, \
exportFormat:      DropDownList { }, \
}, \
exportDescGroup: Group { orientation: 'row', \
exportFormatLabel: StaticText { text: '' }, \
exportFormat:      StaticText { text: 'Requires Twixl Reader 4.2 or newer' }, \
}, \
}, \
buttons: Group { orientation: 'row', alignment: 'right', \
cancelBtn: Button { text:'Cancel', properties: {name:'cancel'} }, \
okBtn:     Button { text:'OK',     properties: {name:'ok'} }, \
} \
}";
            var myDialog = new Window(myRes, 'Export Preferences', undefined, {
                resizable: false,
                closeButton: false
            });
            myDialog.center();
            TMUI.fixLabelWidthsForDialog(myDialog);
            this._configureExportFormat(myDialog);
            if (myDialog.show() === 1) {
                return true;
            } else {
                return false;
            }
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs.selectExportFormat", myException);
        }
    },
    showNewPublication: function () {
        try {
            var enableLegacyOptions = TMPreferences.isRunningLegacyMode();
            var myRes = "dialog { alignChildren: 'fill', text: '', \
publicationPanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Publication Properties',";
            if (enableLegacyOptions) {
                myRes += "\
publicationStyleGroup: Group { orientation: 'row', \
publicationStyleLabel: StaticText { text: 'Style' }, \
publicationStyle:      DropDownList {}, \
},";
            }
            myRes += "\
publicationNameGroup: Group { orientation: 'row', \
publicationNameLabel: StaticText { text: 'Name' }, \
publicationName:      EditText { active: true }, \
}, \
publicationTocStyleGroup: Group { orientation: 'row', \
publicationTocStyleLabel: StaticText { text: 'TOC Viewer' }, \
publicationTocStyle:      DropDownList { }, \
}, \
publicationReadingStyleGroup: Group { orientation: 'row', \
publicationReadingStyleLabel: StaticText { text: 'Reading Style' }, \
publicationReadingStyle:      DropDownList { }, \
}, \
}, \
optionsPanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Publication Options', \
optionsAllowSwiping: Checkbox { text: 'Allow Swiping to Change Articles and Pages', value: true }, \
optionsHorizontalOnly: Checkbox { text: 'Horizontal Swiping Only', value: false }, \
optionsAllowSharing: Checkbox { text: 'Allow Sharing on Social Media', value: true },";
            if (enableLegacyOptions) {
                myRes += "optionsShowStatusBar: Checkbox { text: 'Show Status Bar (iPad Only)', value: false, enabled: false },";
            }
            myRes += "\
}, \
articlePanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Initial Article', \
articleTemplateGroup: Group { orientation: 'row', \
articleTemplateLabel: StaticText { text: 'Template' }, \
articleTemplate:      DropDownList { }, \
}, \
articleNameGroup: Group { orientation: 'row', \
articleNameLabel: StaticText { text: 'File Name' }, \
articleName:      EditText { }, \
}, \
articleTitleGroup: Group { orientation: 'row', \
articleTitleLabel: StaticText { text: 'Title' }, \
articleTitle:      EditText { }, \
}, \
articleTaglineGroup: Group { orientation: 'row', \
articleTaglineLabel: StaticText { text: 'Tag Line' }, \
articleTagline:      EditText { }, \
}, \
}, \
buttons: Group { orientation: 'row', alignment: 'right', \
templatesBtn: Button { text:'Show Templates Folder...' }, \
cancelBtn:    Button { text:'Cancel', properties: {name:'cancel'} }, \
okBtn:        Button { text:'OK',     properties: {name:'ok'} }, \
} \
}";
            var myDialog = new Window(myRes, 'New Publication', undefined, {
                resizable: false,
                closeButton: false
            });
            myDialog.center();
            TMUI.fixLabelWidthsForDialog(myDialog);
            if (enableLegacyOptions) {
                var publicationStyle = myDialog.publicationPanel.publicationStyleGroup.publicationStyle;
            }
            var publicationName = myDialog.publicationPanel.publicationNameGroup.publicationName;
            var publicationTocStyle = myDialog.publicationPanel.publicationTocStyleGroup.publicationTocStyle;
            var publicationReadingStyle = myDialog.publicationPanel.publicationReadingStyleGroup.publicationReadingStyle;
            var optionsAllowSwiping = myDialog.optionsPanel.optionsAllowSwiping;
            var optionsHorizontalOnly = myDialog.optionsPanel.optionsHorizontalOnly;
            var optionsAllowSharing = myDialog.optionsPanel.optionsAllowSharing;
            if (enableLegacyOptions) {
                var optionsShowStatusBar = myDialog.optionsPanel.optionsShowStatusBar;
            }
            var articleTemplate = myDialog.articlePanel.articleTemplateGroup.articleTemplate;
            var articleName = myDialog.articlePanel.articleNameGroup.articleName;
            var articleTitle = myDialog.articlePanel.articleTitleGroup.articleTitle;
            var articleTagline = myDialog.articlePanel.articleTaglineGroup.articleTagline;
            if (enableLegacyOptions) {
                publicationStyle.add('item', 'Use Liquid and Alternate Layouts').selected = true;
                publicationStyle.add('item', 'Publication supports landscape only (legacy)');
                publicationStyle.add('item', 'Publication supports portrait only (legacy)');
                publicationStyle.add('item', 'Publication supports landscape and portrait (legacy)');
            }
            publicationTocStyle.add('item', 'Title Bar Button with Search & Bookmarks').selected = true;
            publicationTocStyle.add('item', 'None');
            publicationReadingStyle.add('item', 'Left to Right').selected = true;
            publicationReadingStyle.add('item', 'Right to Left');
            myDialog.reloadTemplates = function () {
                TMDialogs._setupArticleTemplates(
                    articleTemplate,
                    enableLegacyOptions ? publicationStyle.selection.text : 'Use Liquid and Alternate Layouts',
                    enableLegacyOptions ? optionsShowStatusBar.value : false
                );
            };
            if (enableLegacyOptions) {
                publicationStyle.onChange = function () {
                    if (publicationStyle.selection.text == 'Use Liquid and Alternate Layouts') {
                        optionsShowStatusBar.enabled = false;
                        optionsShowStatusBar.value = false;
                    } else {
                        optionsShowStatusBar.enabled = true;
                    }
                    myDialog.reloadTemplates();
                };
            }
            articleTemplate.onChange = function () {
                TMDialogs._saveArticleTemplate(articleTemplate, enableLegacyOptions ? publicationStyle.selection.text : 'Use Liquid and Alternate Layouts');
            };
            myDialog.buttons.templatesBtn.onClick = function () {
                TMDialogs._showTemplatesFolder(enableLegacyOptions ? publicationStyle.selection.text : 'Use Liquid and Alternate Layouts');
            };
            myDialog.reloadTemplates();
            if (myDialog.show() === 1) {
                if (!publicationName.text || publicationName.text == '') {
                    TMDialogs.error('Please fill in a publication name');
                    return;
                }
                TMBridge.tmNewPublication(
                    publicationName.text,
                    enableLegacyOptions ? publicationStyle.selection.text : 'Use Liquid and Alternate Layouts',
                    publicationTocStyle.selection.text != 'None',
                    publicationReadingStyle.selection.text,
                    optionsAllowSwiping.value,
                    optionsHorizontalOnly.value,
                    enableLegacyOptions ? optionsShowStatusBar.value : false,
                    articleName.text,
                    articleTitle.text,
                    articleTagline.text,
                    articleTemplate.selection.text,
                    optionsAllowSharing.value
                );
            }
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs.showNewPublication", myException);
        }
    },
    showNewArticle: function () {
        try {
            var myRes = "dialog { alignChildren: 'fill', text: '', \
publicationPanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Publication', \
publicationNameGroup: Group { orientation: 'row', \
publicationNameLabel: StaticText { text: 'File Name' }, \
publicationName:      EditText { properties: { readonly: true } }, \
}, \
}, \
articlePanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Article Properties', \
articleTemplateGroup: Group { orientation: 'row', \
articleTemplateLabel: StaticText { text: 'Template' }, \
articleTemplate:      DropDownList { }, \
}, \
articleNameGroup: Group { orientation: 'row', \
articleNameLabel: StaticText { text: 'File Name' }, \
articleName:      EditText { active: true }, \
}, \
articleTitleGroup: Group { orientation: 'row', \
articleTitleLabel: StaticText { text: 'Title' }, \
articleTitle:      EditText { }, \
}, \
articleTaglineGroup: Group { orientation: 'row', \
articleTaglineLabel: StaticText { text: 'Tag Line' }, \
articleTagline:      EditText { }, \
}, \
articleAuthorGroup: Group { orientation: 'row', \
articleAuthorLabel: StaticText { text: 'Author' }, \
articleAuthor:      EditText { }, \
}, \
articleUrlGroup: Group { orientation: 'row', \
articleUrlLabel: StaticText { text: 'Article URL' }, \
articleUrl:      EditText { }, \
}, \
}, \
optionsPanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Article Options', \
optionsShowInScrubber: Checkbox { text: 'Show Article in TOC Viewer', value: true }, \
}, \
buttons: Group { orientation: 'row', alignment: 'right', \
templatesBtn: Button { text:'Show Templates Folder...' }, \
cancelBtn: Button { text:'Cancel', properties: {name:'cancel'} }, \
okBtn:     Button { text:'OK',     properties: {name:'ok'} }, \
} \
}";
            var myDialog = new Window(myRes, 'New Article', undefined, {
                resizable: false,
                closeButton: false
            });
            myDialog.center();
            TMUI.fixLabelWidthsForDialog(myDialog);
            var publicationName = myDialog.publicationPanel.publicationNameGroup.publicationName;
            var articleTemplate = myDialog.articlePanel.articleTemplateGroup.articleTemplate;
            var articleName = myDialog.articlePanel.articleNameGroup.articleName;
            var articleTitle = myDialog.articlePanel.articleTitleGroup.articleTitle;
            var articleTagline = myDialog.articlePanel.articleTaglineGroup.articleTagline;
            var articleAuthor = myDialog.articlePanel.articleAuthorGroup.articleAuthor;
            var articleUrl = myDialog.articlePanel.articleUrlGroup.articleUrl;
            var optionsShowInScrubber = myDialog.optionsPanel.optionsShowInScrubber;
            publicationName.text = TMApplication.currentPublicationName();
            var publicationStyle = 'Use Liquid and Alternate Layouts';
            var showStatusBar = false;
            var publicationOrientations = undefined;
            if (publicationName.text != 'No Publication Open') {
                publicationStyle = app.activeBook.tmPublicationStyle();
                showStatusBar = app.activeBook.tmShowStatusBar();
                publicationOrientations = app.activeBook.tmOrientations();
            }
            myDialog.reloadTemplates = function () {
                TMDialogs._setupArticleTemplates(
                    articleTemplate,
                    publicationStyle,
                    showStatusBar
                );
            };
            articleTemplate.onChange = function () {
                TMDialogs._saveArticleTemplate(articleTemplate, publicationStyle);
            };
            myDialog.buttons.templatesBtn.onClick = function () {
                TMDialogs._showTemplatesFolder(publicationStyle);
            };
            myDialog.reloadTemplates();
            if (myDialog.show() === 1) {
                if (!articleName.text || articleName.text == '') {
                    return;
                }
                if (publicationName.text == 'No Publication Open') {
                    TMBridge.tmNewSingleArticle(
                        articleName.text,
                        articleTitle.text,
                        articleTagline.text,
                        articleAuthor.text,
                        articleUrl.text,
                        articleTemplate.selection.text,
                        optionsShowInScrubber.value,
                        ''
                    );
                } else {
                    TMBridge.tmNewArticle(
                        articleName.text,
                        articleTitle.text,
                        articleTagline.text,
                        articleAuthor.text,
                        articleUrl.text,
                        articleTemplate.selection.text,
                        optionsShowInScrubber.value,
                        publicationStyle == 'Use Liquid and Alternate Layouts',
                        showStatusBar,
                        publicationOrientations,
                        ''
                    );
                }
            }
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs.showNewArticle", myException);
        }
    },
    showPublicationProperties: function () {
        try {
            if (!TMChecks.hasOpenBook()) {
                return;
            }
            var enableLegacyOptions = TMPreferences.isRunningLegacyMode();
            var myRes = "dialog { alignChildren: 'fill', text: '', \
publicationFilePanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Publication', \
publicationFileNameGroup: Group { orientation: 'row', \
publicationFileNameLabel: StaticText { text: 'File Name' }, \
publicationFileName:      EditText { properties: { readonly: true } }, \
}, \
}, \
publicationPanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Publication Properties',"
            if (enableLegacyOptions) {
                myRes += "\
publicationStyleGroup: Group { orientation: 'row', \
publicationStyleLabel: StaticText { text: 'Style' }, \
publicationStyle:      DropDownList {}, \
},";
            }
            myRes += "\
publicationNameGroup: Group { orientation: 'row', \
publicationNameLabel: StaticText { text: 'Name' }, \
publicationName:      EditText { active: true }, \
}, \
publicationTocStyleGroup: Group { orientation: 'row', \
publicationTocStyleLabel: StaticText { text: 'TOC Viewer' }, \
publicationTocStyle:      DropDownList { }, \
}, \
publicationReadingStyleGroup: Group { orientation: 'row', \
publicationReadingStyleLabel: StaticText { text: 'Reading Style' }, \
publicationReadingStyle:      DropDownList { }, \
}, \
}, \
optionsPanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Publication Options', \
optionsAllowSwiping: Checkbox { text: 'Allow Swiping to Change Articles and Pages', value: true }, \
optionsHorizontalOnly: Checkbox { text: 'Horizontal Swiping Only', value: false }, \
optionsAllowSharing: Checkbox { text: 'Allow Sharing on Social Media', value: true },";
            if (enableLegacyOptions) {
                myRes += "optionsShowStatusBar: Checkbox { text: 'Show Status Bar (iPad Only)', value: false, enabled: false },";
            }
            myRes += "\
}, \
buttons: Group { orientation: 'row', alignment: 'right', \
cancelBtn: Button { text:'Cancel', properties: {name:'cancel'} }, \
okBtn:     Button { text:'OK',     properties: {name:'ok'} }, \
} \
}";
            var myDialog = new Window(myRes, 'Publication Properties', undefined, {
                resizable: false,
                closeButton: false
            });
            myDialog.center();
            TMUI.fixLabelWidthsForDialog(myDialog);
            var publicationFileName = myDialog.publicationFilePanel.publicationFileNameGroup.publicationFileName;
            if (enableLegacyOptions) {
                var publicationStyle = myDialog.publicationPanel.publicationStyleGroup.publicationStyle;
            }
            var publicationName = myDialog.publicationPanel.publicationNameGroup.publicationName;
            var publicationTocStyle = myDialog.publicationPanel.publicationTocStyleGroup.publicationTocStyle;
            var publicationReadingStyle = myDialog.publicationPanel.publicationReadingStyleGroup.publicationReadingStyle;
            var optionsAllowSwiping = myDialog.optionsPanel.optionsAllowSwiping;
            var optionsHorizontalOnly = myDialog.optionsPanel.optionsHorizontalOnly;
            var optionsAllowSharing = myDialog.optionsPanel.optionsAllowSharing;
            if (enableLegacyOptions) {
                var optionsShowStatusBar = myDialog.optionsPanel.optionsShowStatusBar;
            }
            publicationFileName.text = TMApplication.currentPublicationName();
            if (enableLegacyOptions) {
                publicationStyle.add('item', 'Use Liquid and Alternate Layouts');
                publicationStyle.add('item', 'Publication supports landscape only (legacy)');
                publicationStyle.add('item', 'Publication supports portrait only (legacy)');
                publicationStyle.add('item', 'Publication supports landscape and portrait (legacy)');
                if (app.activeBook.tmUsesAlternateLayouts() == 1) {
                    publicationStyle.selection = 0;
                } else {
                    var orientations = app.activeBook.tmOrientations();
                    if (orientations == 'landscape') {
                        publicationStyle.selection = 1;
                    } else if (orientations == 'portrait') {
                        publicationStyle.selection = 2;
                    } else {
                        publicationStyle.selection = 3;
                    }
                }
            }
            publicationTocStyle.add('item', 'Title Bar Button with Search & Bookmarks');
            publicationTocStyle.add('item', 'None');
            publicationReadingStyle.add('item', 'Left to Right').selected = true;
            publicationReadingStyle.add('item', 'Right to Left');
            var properties = app.activeBook.tmProperties();
            publicationName.text = TMUtilities.stringWithDefault(properties, 'name', '');
            optionsAllowSwiping.value = TMUtilities.boolWithDefault(properties, 'scrollEnabled', true);
            optionsHorizontalOnly.value = TMUtilities.boolWithDefault(properties, 'horizontalOnly', false);
            optionsAllowSharing.value = TMUtilities.boolWithDefault(properties, 'allowSharing', true);
            if (enableLegacyOptions) {
                optionsShowStatusBar.value = TMUtilities.boolWithDefault(properties, 'showStatusBar', false);
            }
            if (TMUtilities.boolWithDefault(properties, 'scrubber', true)) {
                publicationTocStyle.selection = 0;
            } else {
                publicationTocStyle.selection = 1;
            }
            if (TMUtilities.stringWithDefault(properties, 'readingStyle', 'Left to Right') == 'Right to Left') {
                publicationReadingStyle.selection = 1;
            } else {
                publicationReadingStyle.selection = 0;
            }
            if (myDialog.show() === 1) {
                TMBridge.tmSaveBookProperties(
                    publicationName.text,
                    enableLegacyOptions ? publicationStyle.selection.text : "Use Liquid and Alternate Layouts",
                    publicationTocStyle.selection.text != 'None',
                    publicationReadingStyle.selection.text,
                    optionsAllowSwiping.value,
                    optionsHorizontalOnly.value,
                    enableLegacyOptions ? optionsShowStatusBar.value : false,
                    TMUtilities.stringWithDefault(properties, 'jpegQuality', 'High'),
                    optionsAllowSharing.value
                );
            }
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs.showPublicationProperties", myException);
        }
    },
    showArticleProperties: function () {
        try {
            if (!TMChecks.hasOpenDocument()) {
                return;
            }
            var myRes = "dialog { alignChildren: 'fill', text: '', \
articleFilePanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Article', \
articleFileNameGroup: Group { orientation: 'row', \
articleFileNameLabel: StaticText { text: 'File Name' }, \
articleFileName:      EditText { properties: { readonly: true } }, \
}, \
}, \
articlePanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Article Properties', \
articleTitleGroup: Group { orientation: 'row', \
articleTitleLabel: StaticText { text: 'Title' }, \
articleTitle:      EditText { active: true }, \
}, \
articleTaglineGroup: Group { orientation: 'row', \
articleTaglineLabel: StaticText { text: 'Tag Line' }, \
articleTagline:      EditText { }, \
}, \
articleAuthorGroup: Group { orientation: 'row', \
articleAuthorLabel: StaticText { text: 'Author' }, \
articleAuthor:      EditText { }, \
}, \
articleUrlGroup: Group { orientation: 'row', \
articleUrlLabel: StaticText { text: 'Article URL' }, \
articleUrl:      EditText { }, \
}, \
}, \
articlePlaylistPanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Background Music', \
articlePlaylistGroup: Group { orientation: 'row', \
articlePlaylistLabel: StaticText { text: 'Playlist' }, \
articlePlaylist:      EditText { properties: { hasButton: true }}, \
articlePlaylistButton:      Button { text: '...', name:'playlistButton' }, \
}, \
}, \
optionsPanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Article Options', \
optionsShowInScrubber: Checkbox { text: 'Show Article in TOC Viewer', value: true }, \
}, \
buttons: Group { orientation: 'row', alignment: 'right', \
cancelBtn: Button { text:'Cancel', properties: {name:'cancel'} }, \
okBtn:     Button { text:'OK',     properties: {name:'ok'} }, \
} \
}";
            var myDialog = new Window(myRes, 'Article Properties', undefined, {
                resizable: false,
                closeButton: false
            });
            myDialog.center();
            var articleFileName = myDialog.articleFilePanel.articleFileNameGroup.articleFileName;
            var articleTitle = myDialog.articlePanel.articleTitleGroup.articleTitle;
            var articleTagline = myDialog.articlePanel.articleTaglineGroup.articleTagline;
            var articleAuthor = myDialog.articlePanel.articleAuthorGroup.articleAuthor;
            var articleUrl = myDialog.articlePanel.articleUrlGroup.articleUrl;
            var articlePlaylist = myDialog.articlePlaylistPanel.articlePlaylistGroup.articlePlaylist;
            var articlePlaylistButton = myDialog.articlePlaylistPanel.articlePlaylistGroup.articlePlaylistButton;
            var optionsShowInScrubber = myDialog.optionsPanel.optionsShowInScrubber;
            var properties = app.activeDocument.tmProperties();
            articleFileName.text = app.activeDocument.tmArticleName();
            articleTitle.text = TMUtilities.stringWithDefault(properties, 'title', '');
            articleTagline.text = TMUtilities.stringWithDefault(properties, 'tagline', '');
            articleAuthor.text = TMUtilities.stringWithDefault(properties, 'author', '');
            articleUrl.text = TMUtilities.stringWithDefault(properties, 'articleURL', '');
            articlePlaylist.text = TMUtilities.stringWithDefault(properties, 'backgroundMusicPlaylist', '');
            optionsShowInScrubber.value = TMUtilities.boolWithDefault(properties, 'showInScrubber', true);
            articlePlaylistButton.onClick = function () {
                var documentPath = app.activeDocument.filePath.fsName;
                var rootFolder = Folder.myDocuments.fsName;
                if (articlePlaylist.text) {
                    rootFolder = articlePlaylist.text;
                }
                rootFolder = TMFiles.pathRelToAbs(documentPath, rootFolder);
                var chosenFolder = TMFiles.browseForFolder("Select a folder with .mp3 files", rootFolder);
                if (chosenFolder) {
                    articlePlaylist.text = TMFiles.pathAbsToRel(documentPath, chosenFolder);
                }
            }
            TMUI.fixLabelWidthsForDialog(myDialog);
            if (myDialog.show() === 1) {
                TMBridge.tmSaveArticleProperties(
                    articleTitle.text,
                    articleTagline.text,
                    articleAuthor.text,
                    articleUrl.text,
                    optionsShowInScrubber.value,
                    articlePlaylist.text
                );
            }
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs.showArticleProperties", myException);
        }
    },
    showSharePublication: function () {
        try {
            if (!TMChecks.hasOpenBook()) {
                return;
            }
            if (!TMChecks.bookHasDocuments()) {
                return;
            }
            if (!TMChecks.allDocumentsAreSaved()) {
                return;
            }
            if (!TMChecks.isHelperRunning()) {
                return;
            }
            var fileTitle = 'Publication';
            var fileName = TMApplication.currentPublicationName();
            var preflightResult = TMPreflighter.preflight([undefined, undefined, 'share_publication']);
            if (!preflightResult) {
                if (!TMDialogs.showPreflightResult('publication')) {
                    return;
                }
            }
            var shareDialog = TMDialogs._createShareDialog('Share Publication', fileTitle, fileName);
            if (shareDialog.show() === 1) {
                var sharingFrom = shareDialog.sharingPanel.sharingFromGroup.sharingFrom;
                var sharingTo = shareDialog.sharingPanel.sharingToGroup.sharingTo;
                var sharingMessage = shareDialog.sharingPanel.sharingMessageGroup.sharingMessage;
                TMBridge.tmShare([sharingFrom.text, sharingTo.text, sharingMessage.text]);
            }
            TMDialogs._saveSharingPreferences(shareDialog);
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs.showSharePublication", myException);
        }
    },
    showShareArticle: function () {
        try {
            if (!TMChecks.hasOpenDocument()) {
                return;
            }
            if (!TMChecks.isDocumentSaved()) {
                return;
            }
            if (!TMChecks.isHelperRunning()) {
                return;
            }
            var fileTitle = 'Article';
            var fileName = app.activeDocument.tmArticleName();
            var filePath = app.activeDocument.fullName.fsName;
            var preflightResult = TMPreflighter.preflightArticle([filePath, undefined, 'share_article']);
            if (!preflightResult) {
                if (!TMDialogs.showPreflightResult('article')) {
                    return;
                }
            }
            var shareDialog = TMDialogs._createShareDialog('Share Article', fileTitle, fileName);
            if (shareDialog.show() === 1) {
                var sharingFrom = shareDialog.sharingPanel.sharingFromGroup.sharingFrom;
                var sharingTo = shareDialog.sharingPanel.sharingToGroup.sharingTo;
                var sharingMessage = shareDialog.sharingPanel.sharingMessageGroup.sharingMessage;
                TMBridge.tmShare([sharingFrom.text, sharingTo.text, sharingMessage.text, filePath]);
            }
            TMDialogs._saveSharingPreferences(shareDialog);
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs.showShareArticle", myException);
        }
    },
    _createShareDialog: function (title, fileTitle, fileNameText) {
        try {
            var myRes = "dialog { alignChildren: 'fill', text: '', \
filePanel: Panel { orientation: 'column', alignChildren: 'left', text: '', \
fileNameGroup: Group { orientation: 'row', \
fileNameLabel: StaticText { text: 'File Name' }, \
fileName:      EditText { properties: { readonly: true } }, \
}, \
}, \
sharingPanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Sharing Options', \
sharingFromGroup: Group { orientation: 'row', \
sharingFromLabel: StaticText { text: 'Your Email' }, \
sharingFrom:      EditText { active: true }, \
}, \
sharingToGroup: Group { orientation: 'row', \
sharingToLabel: StaticText { text: 'Share With' }, \
sharingTo:      EditText { }, \
}, \
sharingMessageGroup: Group { orientation: 'row', alignChildren: 'top', \
sharingMessageLabel: StaticText { text: 'Message' }, \
sharingMessage:      EditText { properties: { multiline: true } }, \
}, \
}, \
exportFormatPanel: Panel { orientation: 'column', alignChildren: 'left', text: '', \
exportFormatGroup: Group { orientation: 'row', \
exportFormatLabel: StaticText { text: 'Export As' }, \
exportFormat:      DropDownList { }, \
}, \
exportDescGroup: Group { orientation: 'row', \
exportFormatLabel: StaticText { text: '' }, \
exportFormat:      StaticText { text: 'Requires Twixl Reader 4.2 or newer' }, \
}, \
}, \
buttons: Group { orientation: 'row', alignment: 'right', \
cancelBtn: Button { text:'Cancel', properties: {name:'cancel'} }, \
okBtn:     Button { text:'OK',     properties: {name:'ok'} }, \
} \
}";
            var myDialog = new Window(myRes, title, undefined, {
                resizable: false,
                closeButton: false
            });
            myDialog.center();
            var filePanel = myDialog.filePanel;
            var fileName = myDialog.filePanel.fileNameGroup.fileName;
            var sharingFrom = myDialog.sharingPanel.sharingFromGroup.sharingFrom;
            var sharingTo = myDialog.sharingPanel.sharingToGroup.sharingTo;
            var sharingMessage = myDialog.sharingPanel.sharingMessageGroup.sharingMessage;
            filePanel.text = fileTitle;
            fileName.text = fileNameText;
            sharingFrom.text = TMPreferences.readObject('share_from', '');
            sharingTo.text = TMPreferences.readObject('share_to', '');
            sharingMessage.minimumSize.height = 120;
            TMUI.fixLabelWidthsForDialog(myDialog);
            this._configureExportFormat(myDialog);
            return myDialog;
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs._createShareDialog", myException);
        }
    },
    _saveSharingPreferences: function (myDialog) {
        try {
            var sharingFrom = myDialog.sharingPanel.sharingFromGroup.sharingFrom;
            var sharingTo = myDialog.sharingPanel.sharingToGroup.sharingTo;
            TMLogger.info('Saving: ' + sharingFrom.text);
            TMLogger.info('Saving: ' + sharingTo.text);
            TMPreferences.saveObject('share_from', sharingFrom.text);
            TMPreferences.saveObject('share_to', sharingTo.text);
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs._saveSharingPreferences", myException);
        }
    },
    showPreviewPublication: function () {
        try {
            if (!TMChecks.hasOpenBook()) {
                return;
            }
            if (!TMChecks.bookHasDocuments()) {
                return;
            }
            if (!TMChecks.allDocumentsAreSaved()) {
                return;
            }
            if (!TMChecks.isHelperRunning()) {
                return;
            }
            try {
                var viewer = this.selectPreviewHost('Preview Publication');
                if (!viewer) {
                    return;
                }
            } catch (e) {
                TMLogger.exception("TMDailogs.showPreviewPublication", e + ' (tm_dialogs:884)');
                TMDialogs.error(e);
                return;
            }
            TMLogger.info('Previewing to: ' + viewer.toSource());
            var previewHost = undefined;
            var previewOnDevice = false;
            var preflightAction = 'preview_simulator';
            if (viewer.hasOwnProperty('ipAddress')) {
                previewHost = viewer['ipAddress'];
                previewOnDevice = true;
                preflightAction = 'preview_publication';
            }
            var deviceModel = viewer['deviceType'];
            var specifier = viewer['udid'];
            try {
                if (!TMDialogs.verifyPreviewHost()) {
                    return;
                }
            } catch (e) {
                TMLogger.exception("TMDailogs.showPreviewPublication", e + ' (tm_dialogs:908)');
                TMDialogs.error(e);
                return;
            }
            deviceModel = TMPreferences.readObject('preview.deviceModel');
            var preflightResult = TMPreflighter.preflight([undefined, deviceModel, preflightAction]);
            if (!preflightResult) {
                if (!TMDialogs.showPreflightResult('publication')) {
                    return;
                }
            }
            TMBridge.tmPreview([undefined, previewOnDevice, deviceModel, previewHost, specifier]);
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs.showPreviewPublication", myException);
        }
    },
    showPreviewArticle: function () {
        try {
            if (!TMChecks.hasOpenDocument()) {
                return;
            }
            if (!TMChecks.isDocumentSaved()) {
                return;
            }
            if (!TMChecks.isHelperRunning()) {
                return;
            }
            try {
                var viewer = this.selectPreviewHost('Preview Article');
                if (!viewer) {
                    return;
                }
            } catch (e) {
                TMLogger.exception("TMDailogs.showPreviewArticle", e + ' (tm_dialogs:952)');
                TMDialogs.error(e);
                return;
            }
            TMLogger.info('Previewing to: ' + viewer.toSource());
            var previewHost = undefined;
            var previewOnDevice = false;
            var preflightAction = 'preview_simulator';
            if (viewer.hasOwnProperty('ipAddress')) {
                previewHost = viewer['ipAddress'];
                previewOnDevice = true;
                preflightAction = 'preview_publication';
            }
            var deviceModel = viewer['deviceType'];
            var specifier = viewer['udid'];
            var articlePath = app.activeDocument.filePath.fsName + '/' + app.activeDocument.name;
            try {
                if (!TMDialogs.verifyPreviewHost()) {
                    return;
                }
            } catch (e) {
                TMLogger.exception("TMDailogs.showPreviewArticle", e + ' (tm_dialogs:978)');
                TMDialogs.error(e);
                return;
            }
            deviceModel = TMPreferences.readObject('preview.deviceModel');
            var preflightResult = TMPreflighter.preflightArticle([articlePath, deviceModel, preflightAction]);
            if (!preflightResult) {
                if (!TMDialogs.showPreflightResult('article')) {
                    return;
                }
            }
            TMBridge.tmPreview([articlePath, previewOnDevice, deviceModel, previewHost, specifier]);
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs.showPreviewArticle", myException);
        }
    },
    selectPreviewHost: function (title) {
        var viewers = undefined;
        try {
            viewers = TMHelper.previewDevices();
        } catch (myException) {
            TMLogger.error(myException);
            if (File.fs == "Windows") {
                TMDialogs.alert("Failed to get the list of viewers.");
            } else {
                TMDialogs.alert("Failed to get the list of viewers.\n\nPlease make sure you have started Xcode once to install the additional components.");
            }
            return;
        }
        var myRes = "dialog { alignChildren: 'fill', text: ''}";
        var myDialog = new Window(myRes, title, undefined, {
            resizable: false,
            closeButton: false
        });
        var viewerlist = myDialog.add('ListBox', undefined, '', {
            numberOfColumns: 4,
            showHeaders: true,
            columnTitles: ['Device Name', 'Device Type', 'Twixl Version', 'IP Address'],
            columnWidths: [200, 155, 90, 155],
            minimumSize: ['620, 420']
        });
        var exportFormatPanel = myDialog.add('Panel', undefined, '', {
            'orientation': 'column',
            'alignChildren': 'left'
        });
        var exportFormatGroup = exportFormatPanel.add('Group', undefined, '', {
            'orientation': 'row'
        });
        var exportFormatLabel = exportFormatGroup.add('StaticText', undefined, 'Export As', {});
        var exportFormat = exportFormatGroup.add('DropDownList');
        var exportDescGroup = exportFormatPanel.add('Group', undefined, '', {
            'orientation': 'row',
        });
        var exportFormatLabelDesc = exportDescGroup.add('StaticText', undefined, '', {});
        var exportFormatDesc = exportDescGroup.add('StaticText', undefined, 'Requires Twixl Reader 4.2 or newerfdsqfqsdfqsfqsdfqdsfqsdfdqsfdqsfdsqf', {});
        var buttons = myDialog.add('Group', undefined, '', {
            'orientation': 'row'
        });
        var manualBtn = buttons.add('Button', undefined, 'Use IP Address...');
        var reloadBtn = buttons.add('Button', undefined, 'Reload');
        var cancelBtn = buttons.add('Button', undefined, 'Cancel', {
            'name': 'Cancel'
        });
        var okBtn = buttons.add('Button', undefined, 'OK', {
            'name': 'OK'
        });
        myDialog.center();
        buttons.alignChildren = 'right';
        buttons.alignment = 'right';
        var previousSelection = TMPreferences.readObject('preview.host', '');
        if (!TMUtilities.objectIsEmpty(viewers.viewers)) {
            for (var i in viewers.viewers) {
                var viewer = viewers.viewers[i];
                var viewerItem = viewerlist.add('item', viewer['deviceName']);
                if (viewer['ipAddress']) {
                    viewerItem.subItems[0].text = TMDeviceUtilities.friendlyDeviceType(viewer['deviceType']);
                    viewerItem.subItems[1].text = viewer['twixlVersion'];
                    viewerItem.subItems[2].text = viewer['ipAddress'];
                } else {
                    viewerItem.subItems[0].text = TMDeviceUtilities.friendlyDeviceType(viewer['deviceType']);
                    viewerItem.subItems[1].text = TMVersion.version;
                    viewerItem.subItems[2].text = 'iOS Simulator';
                }
                if (previousSelection == viewer['ipAddress'] || previousSelection == viewer['udid']) {
                    viewerItem.selected = true;
                }
            }
        }
        if (!viewerlist.selection && viewerlist.items.length > 0) {
            viewerlist.selection = 0;
        }
        TMUI.fixLabelWidthsForDialog(myDialog);
        this._configureExportFormat(myDialog, exportFormat, exportFormatDesc);
        var viewer = undefined;
        manualBtn.onClick = function () {
            var previousHost = '';
            if (TMPreferences.readObject('preview.mode') == 'device') {
                previousHost = TMPreferences.readObject('preview.host');
            }
            var ipAddress = prompt('Enter the IP address of the Twixl Viewer', previousHost);
            if (ipAddress) {
                TMPreferences.saveObject('preview.host', ipAddress);
                TMPreferences.saveObject('preview.mode', 'device');
                viewer = {
                    'ipAddress': ipAddress
                };
                myDialog.close();
            }
        };
        reloadBtn.onClick = function () {
            var viewers = TMHelper.previewDevices();
            var selection = viewerlist.selection;
            selectedViewer = viewers.viewers[selection.index];
            if (selectedViewer['ipAddress']) {
                selection = selectedViewer['ipAddress'];
            } else {
                selection = selectedViewer['udid'];
            }
            TMLogger.info("S " + selectedViewer['ipAddress'] + ", " + selectedViewer['udid']);
            viewerlist.removeAll();
            if (!TMUtilities.objectIsEmpty(viewers.viewers)) {
                for (var i in viewers.viewers) {
                    var viewer = viewers.viewers[i];
                    var viewerItem = viewerlist.add('item', viewer['deviceName']);
                    if (viewer['ipAddress']) {
                        viewerItem.subItems[0].text = TMDeviceUtilities.friendlyDeviceType(viewer['deviceType']);
                        viewerItem.subItems[1].text = viewer['twixlVersion'];
                        viewerItem.subItems[2].text = viewer['ipAddress'];
                    } else {
                        viewerItem.subItems[0].text = TMDeviceUtilities.friendlyDeviceType(viewer['deviceType']);
                        viewerItem.subItems[1].text = TMVersion.version;
                        viewerItem.subItems[2].text = 'iOS Simulator';
                    }
                    if (selection == viewer['ipAddress'] || selection == viewer['udid']) {
                        viewerItem.selected = true;
                        TMLogger.info("W " + viewer['ipAddress'] + ", " + viewer['udid']);
                    } else {
                        TMLogger.info("V " + viewer['ipAddress'] + ", " + viewer['udid']);
                    }
                }
            }
        }
        if (myDialog.show() === 1) {
            var selection = viewerlist.selection;
            viewer = viewers.viewers[selection.index];
            if (viewer['ipAddress']) {
                TMPreferences.saveObject('preview.mode', 'device');
                TMPreferences.saveObject('preview.host', viewer['ipAddress']);
            } else {
                TMPreferences.saveObject('preview.mode', 'mac');
                TMPreferences.saveObject('preview.host', viewer['udid']);
            }
            TMPreferences.saveObject('preview.deviceModel', viewer['deviceType']);
        }
        return viewer;
    },
    verifyPreviewHost: function () {
        var previewMode = TMPreferences.readObject('preview.mode');
        var previewHost = TMPreferences.readObject('preview.host');
        if (previewMode == 'mac' && File.fs != 'Macintosh') {
            throw 'Preview using the iOS Simulator only works on Macintosh.';
        }
        if (previewMode == 'device') {
            if (!previewHost) {
                throw 'No preview device IP address was given.';
            }
            TMProgressBar.create('Preview', 'Verifying preview host: ' + previewHost, 1);
            TMLogger.info('Verifying preview host: ' + previewHost);
            var timestamp = new Date().getTime();
            var deviceData = TMURL.get('http://' + previewHost + ':' + kPORT_VIEWER + '/check?' + timestamp, false, false, 5);
            TMProgressBar.close();
            if (deviceData == '') {
                throw 'Failed to preview on device "' + previewHost + '".\n\nMake sure the Twixl Viewer application is running on the device.';
            } else {
                TMLogger.debug(deviceData);
            }
            var deviceDataParts = deviceData.split('|');
            var deviceModel = deviceDataParts[0];
            var deviceVersion = '3.0';
            var deviceMode = 'push';
            if (deviceDataParts.length > 1) {
                deviceVersion = deviceDataParts[1];
            }
            if (deviceDataParts.length > 2) {
                deviceMode = deviceDataParts[2]
            }
            var viewerVersion = TMVersionUtilities.getMajorVersion(deviceVersion);
            var pluginVersion = TMVersionUtilities.getMajorVersion(TMVersion.version);
            TMLogger.info('Twixl Viewer: ' + deviceModel + ' version ' + deviceVersion + ' (major version: ' + viewerVersion + ', plugin version: ' + pluginVersion + ', mode: ' + deviceMode + ')');
            if (deviceModel != "ipad" && deviceModel != "ipad-retina" && deviceModel != "android10" && deviceModel != "android7" && deviceModel != 'phone_s' && deviceModel != 'phone_m' && deviceModel != 'phone_l') {
                throw "The device on \"" + previewHost + "\" is not a Twixl Viewer application.";
            }
            if (viewerVersion < pluginVersion) {
                var result = TMDialogs.confirm("Are you sure you want to continue?\n\nThis publication is created with a newer version of the plugin (" + TMVersion.version + ") compared to the Twixl Viewer version on your device (" + deviceVersion + "). Not all things might work as expected.");
                if (!result) {
                    return false;
                }
            }
            TMPreferences.saveObject('preview.deviceModel', deviceModel);
            TMPreferences.saveObject('preview.deviceMode', deviceMode);
        }
        return true;
    },
    showExportPublication: function () {
        try {
            if (!TMChecks.hasOpenBook()) {
                return;
            }
            if (!TMChecks.bookHasDocuments()) {
                return;
            }
            if (!TMChecks.allDocumentsAreSaved()) {
                return;
            }
            if (!TMChecks.isHelperRunning()) {
                return;
            }
            var preflightResult = TMPreflighter.preflight([undefined, undefined, 'export_publication']);
            if (!preflightResult) {
                if (!TMDialogs.showPreflightResult('publication')) {
                    return;
                }
            }
            var exportFolder = TMBridge.tmExportLocation();
            if (!exportFolder) {
                return;
            }
            var exportFolderInstance = new Folder(exportFolder);
            if (exportFolderInstance.exists) {
                var result = TMDialogs.confirm('Are you sure you want to overwrite "' + TMUtilities.decodeURI(exportFolderInstance.name) + '"?');
                if (!result) {
                    return;
                }
            }
            if (!this.selectExportFormat()) {
                return;
            }
            var exportResult = TMBridge.tmExport([exportFolder]);
            if (!exportResult) {
                return;
            }
            if (TMPluginCore.engine != 'html') {
                TMDialogs._exportComplete(exportFolderInstance.fsName);
            }
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs.showExportPublication", myException);
        }
    },
    showExportAsArticles: function () {
        try {
            if (!TMChecks.hasOpenBook()) {
                return;
            }
            if (!TMChecks.bookHasDocuments()) {
                return;
            }
            if (!TMChecks.allDocumentsAreSaved()) {
                return;
            }
            if (!TMChecks.isHelperRunning()) {
                return;
            }
            TMLogger.visibleFiles();
            var preflightResult = TMPreflighter.preflight([undefined, undefined, 'export_publication']);
            if (!preflightResult) {
                if (!TMDialogs.showPreflightResult('publication')) {
                    return;
                }
            }
            TMLogger.visibleFiles();
            var exportFolder = TMBridge.tmExportArticlesLocation();
            if (!exportFolder) {
                return;
            }
            var exportFolderInstance = new Folder(exportFolder);
            if (exportFolderInstance.exists) {
                var result = TMDialogs.confirm('Are you sure you want to overwrite "' + TMUtilities.decodeURI(exportFolderInstance) + '"?');
                if (!result) {
                    return;
                }
                TMLogger.info('Overwriting: ' + exportFolderInstance.fsName);
                TMFiles.deleteFolderAndContents(exportFolderInstance.fsName);
            }
            if (!this.selectExportFormat()) {
                return;
            }
            TMTimer.start("export", "Starting new publication as articles export to: " + exportFolderInstance.fsName);
            exportFolderInstance.create();
            var myBook = app.activeBook;
            var myBookContents = TMUtilities.collectionToArray(myBook.bookContents);
            var myArticleCount = myBookContents.length;
            var myTotalProgressSteps = myArticleCount;
            TMProgressBar.create("Export Publication As Articles", 'Exporting publication...', myTotalProgressSteps);
            try {
                var myBookContents = TMUtilities.collectionToArray(myBook.bookContents);
                var myBookContentsCount = myBookContents.length;
                for (var i = 0; i < myBookContentsCount; i++) {
                    var myFile = myBookContents[i].fullName;
                    TMProgressBar.updateLabel(TMUtilities.decodeURI(myFile.name));
                    var myArticlePath = myFile.fsName;
                    var myOutputPath = exportFolderInstance.fsName + '/' + TMFiles.getBaseName(TMUtilities.decodeURI(myFile.name)) + '.article';
                    TMLogger.info("Exporting: " + myArticlePath);
                    TMLogger.info("       To: " + myOutputPath);
                    var suffix = (myBookContentsCount > 1) ? 'articles' : 'article';
                    TMProgressBar.updateTitle("Exporting article " + (i + 1) + " of " + myBookContentsCount + " " + suffix);
                    app.doScript(
                        TwixlPublisherPluginAPI.exportArticleInternal,
                        ScriptLanguage.JAVASCRIPT,
                        [myArticlePath, myOutputPath],
                        UndoModes.ENTIRE_SCRIPT
                    );
                    TMLogger.visibleFiles();
                }
            } catch (myException) {
                TMLogger.exception('TMExporter.showExportAsArticles', myException + ' (tm_dialogs:1371)');
                TMProgressBar.close();
                TMTimer.printElapsed('export', 'Export duration');
                TMStackTrace.addToStack('TMExporter.showExportAsArticles', myException);
            }
            TMProgressBar.close();
            TMApplication.deselectAllObjects();
            TMLogger.info('Publication export as articles was successful');
            TMTimer.printElapsed('export', 'Export duration');
            TMLogger.visibleFiles();
            if (TMPluginCore.engine != 'html') {
                TMDialogs._exportComplete(exportFolderInstance.fsName);
            }
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs.showExportAsArticles", myException);
        }
    },
    showExportArticle: function () {
        try {
            if (!TMChecks.hasOpenDocument()) {
                return;
            }
            if (!TMChecks.isDocumentSaved()) {
                return;
            }
            if (!TMChecks.isHelperRunning()) {
                return;
            }
            var articlePath = app.activeDocument.filePath.fsName + '/' + app.activeDocument.name;
            var preflightResult = TMPreflighter.preflightArticle([articlePath, undefined, 'export_article']);
            if (!preflightResult) {
                if (!TMDialogs.showPreflightResult('article')) {
                    return;
                }
            }
            var exportFolder = TMBridge.tmExportArticleLocation(articlePath);
            if (!exportFolder) {
                return;
            }
            var exportFolderInstance = new Folder(exportFolder);
            if (exportFolderInstance.exists) {
                var result = TMDialogs.confirm('Are you sure you want to overwrite "' + TMUtilities.decodeURI(exportFolderInstance.name) + '"?');
                if (!result) {
                    return;
                }
            }
            if (!this.selectExportFormat()) {
                return;
            }
            var exportResult = TMBridge.tmExportArticle([articlePath, exportFolder]);
            if (!exportResult) {
                return;
            }
            if (TMPluginCore.engine != 'html') {
                TMDialogs._exportComplete(exportFolderInstance.fsName);
            }
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs.showExportArticle", myException);
        }
    },
    _exportComplete: function (exportFolderPath) {
        var exportFolderInstance = new Folder(exportFolderPath);
        if (Folder.fs == 'Macintosh') {
            var result = TMDialogs.confirm('Finished exporting "' + TMUtilities.decodeURI(exportFolderInstance.name) + '".\n\nDo you want to open the export in the Twixl Publisher application?');
            if (!result) {
                return;
            }
            TMLogger.info("Opening with Twixl Publisher: " + exportFolderInstance.fsName);
            var exportFile = new File(exportFolderInstance.fsName);
            exportFile.execute();
        }
        if (Folder.fs == 'Windows') {
            TMDialogs.alert('Finished exporting "' + TMUtilities.decodeURI(exportFolderInstance.name) + '".');
        }
    },
    showPreflightResult: function (preflightType) {
        try {
            var warningCount = TMPreflighter.warningCount();
            var errorCount = TMPreflighter.errorCount();
            if (warningCount == 0 && errorCount == 0) {
                TMLogger.info('not showing preflight dialogs');
                return true;
            }
            var myRes = "dialog { alignChildren: 'fill', text: '', \
results: TreeView { alignment: 'fill' }, \
buttons: Group { orientation: 'row', alignment: 'right', \
cancelBtn: Button { text:'Cancel', alignment: 'right', properties: {name:'cancel'} }, \
okBtn:     Button { text:'Continue...', alignment: 'right', properties: {name:'ok'} }, \
} \
}";
            var myDialog = new Window(myRes, 'Preflight Result', undefined, {
                resizeable: true,
                closeButton: false
            });
            myDialog.onResizing = myDialog.onResize = function () {
                this.layout.resize();
            }
            myDialog.onShow = function () {
                myDialog.minimumSize = myDialog.size;
                myDialog.buttons.maximumSize = myDialog.buttons.size;
            }
            myDialog.results.minimumSize = [640, 280];
            myDialog.results.alignment = ['fill', 'fill'];
            if (TMPreflighter.errors && errorCount > 0) {
                var errors = myDialog.results.add('node', 'Preflight Errors (' + errorCount + ')');
                for (var myCategory in TMPreflighter.errors) {
                    if (preflightType == 'publication') {
                        var errorCategory = errors.add('node', myCategory + ' (' + TMPreflighter.errors[myCategory].length + ')');
                        for (var i = 0; i < TMPreflighter.errors[myCategory].length; i++) {
                            var errorMessage = TMPreflighter.errors[myCategory][i];
                            errorCategory.add('item', errorMessage);
                        };
                        errorCategory.expanded = true;
                    } else {
                        for (var i = 0; i < TMPreflighter.errors[myCategory].length; i++) {
                            var errorMessage = TMPreflighter.errors[myCategory][i];
                            errors.add('item', errorMessage);
                        };
                    }
                }
                errors.expanded = true;
                myDialog.buttons.cancelBtn.hide();
                myDialog.buttons.okBtn.text = 'OK';
            } else {
                myDialog.buttons.okBtn.show();
            }
            if (TMPreflighter.warnings && warningCount > 0) {
                var warnings = myDialog.results.add('node', 'Preflight Warnings (' + warningCount + ')');
                for (var myCategory in TMPreflighter.warnings) {
                    if (preflightType == 'publication') {
                        var warningCategory = warnings.add('node', myCategory + ' (' + TMPreflighter.warnings[myCategory].length + ')');
                        for (var i = 0; i < TMPreflighter.warnings[myCategory].length; i++) {
                            var warningMessage = TMPreflighter.warnings[myCategory][i];
                            warningCategory.add('item', warningMessage);
                        };
                        warningCategory.expanded = true;
                    } else {
                        for (var i = 0; i < TMPreflighter.warnings[myCategory].length; i++) {
                            var warningMessage = TMPreflighter.warnings[myCategory][i];
                            warnings.add('item', warningMessage);
                        };
                    }
                }
                warnings.expanded = true;
            }
            var result = myDialog.show();
            if (result == 1) {
                if (TMPreflighter.errors && TMPreflighter.errorCount() > 0) {
                    return false;
                } else {
                    return true;
                }
            } else {
                return false;
            }
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs.showPreflightResult", myException);
        }
    },
    showAlternateLayouts: function () {
        try {
            if (app.books.length > 0) {
                if (!TMChecks.hasOpenBook()) {
                    return;
                }
                if (!TMChecks.bookHasDocuments()) {
                    return;
                }
            } else {
                if (!TMChecks.hasOpenDocument()) {
                    return;
                }
            }
            if (app.books.length > 0) {
                var articleCount = app.activeBook.tmArticleCount();
                TMProgressBar.create('Create Alternate Layouts', 'Getting the list of alternate layouts', articleCount);
                var alternateLayouts = TMAlternateLayouts.publicationLayouts(app.activeBook, true);
                TMProgressBar.close();
            } else {
                var articleCount = 1;
                var alternateLayouts = TMAlternateLayouts.documentLayouts(app.activeDocument);
            }
            var myRes = "dialog { alignChildren: 'fill', text: '', \
filePanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Publication', \
fileNameGroup: Group { orientation: 'row', \
fileNameLabel: StaticText { text: 'File Name' }, \
fileName:      EditText { properties: { readonly: true } }, \
}, \
}, \
layoutPanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Select the alternate layout(s) to create', \
chkIPadH: Checkbox { text: 'Create \"iPad H\"', value: true }, \
chkIPadV: Checkbox { text: 'Create \"iPad V\"', value: true }, \
chkAndroid10H: Checkbox { text: 'Create \"Android 10\" H\"', value: true }, \
chkAndroid10V: Checkbox { text: 'Create \"Android 10\" V\"', value: true }, \
chkAndroid7H: Checkbox { text: 'Create \"Kindle Fire/Nook H\" (Android 7\")', value: true }, \
chkAndroid7V: Checkbox { text: 'Create \"Kindle Fire/Nook V\" (Android 7\")', value: true }, \
chkPhoneH: Checkbox { text: 'Create \"Phone H\"', value: true }, \
chkPhoneV: Checkbox { text: 'Create \"Phone V\"', value: true }, \
}, \
optionsPanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Options', \
optionsLinkStories: Checkbox { text: 'Link Stories', value: true }, \
optionsCopyTextStyles: Checkbox { text: 'Copy Text Styles to New Style Group', value: true }, \
}, \
buttons: Group { orientation: 'row', alignment: 'right', \
cancelBtn: Button { text:'Cancel', properties: {name:'cancel'} }, \
okBtn:     Button { text:'OK',     properties: {name:'ok'} }, \
} \
}";
            var myDialog = new Window(myRes, 'Create Alternate Layouts', undefined, {
                resizable: false,
                closeButton: false
            });
            myDialog.center();
            myDialog.preferredSize.width = 380;
            if (app.books.length > 0 && app.activeBook) {
                var hasBook = true;
                myDialog.filePanel.text = 'Publication';
                myDialog.filePanel.fileNameGroup.fileName.text = app.activeBook.name;
            } else {
                var hasBook = false;
                myDialog.filePanel.text = 'Article';
                myDialog.filePanel.fileNameGroup.fileName.text = app.activeDocument.name;
            }
            TMDialogs._setAlternateLayoutCheckbox(myDialog.layoutPanel.chkIPadH, hasBook, 'iPad H', alternateLayouts, articleCount);
            TMDialogs._setAlternateLayoutCheckbox(myDialog.layoutPanel.chkIPadV, hasBook, 'iPad V', alternateLayouts, articleCount);
            TMDialogs._setAlternateLayoutCheckbox(myDialog.layoutPanel.chkAndroid10H, hasBook, 'Android 10" H', alternateLayouts, articleCount);
            TMDialogs._setAlternateLayoutCheckbox(myDialog.layoutPanel.chkAndroid10V, hasBook, 'Android 10" V', alternateLayouts, articleCount);
            TMDialogs._setAlternateLayoutCheckbox(myDialog.layoutPanel.chkAndroid7H, hasBook, 'Kindle Fire/Nook H', alternateLayouts, articleCount);
            TMDialogs._setAlternateLayoutCheckbox(myDialog.layoutPanel.chkAndroid7V, hasBook, 'Kindle Fire/Nook V', alternateLayouts, articleCount);
            TMDialogs._setAlternateLayoutCheckbox(myDialog.layoutPanel.chkPhoneH, hasBook, 'Phone H', alternateLayouts, articleCount);
            TMDialogs._setAlternateLayoutCheckbox(myDialog.layoutPanel.chkPhoneV, hasBook, 'Phone V', alternateLayouts, articleCount);
            TMUI.fixLabelWidthsForDialog(myDialog);
            if (myDialog.show() === 1) {
                var requestedLayouts = [];
                if (myDialog.layoutPanel.chkIPadH.value) {
                    requestedLayouts.push('iPad H');
                }
                if (myDialog.layoutPanel.chkIPadV.value) {
                    requestedLayouts.push('iPad V');
                }
                if (myDialog.layoutPanel.chkAndroid10H.value) {
                    requestedLayouts.push('Android 10" H');
                }
                if (myDialog.layoutPanel.chkAndroid10V.value) {
                    requestedLayouts.push('Android 10" V');
                }
                if (myDialog.layoutPanel.chkAndroid7H.value) {
                    requestedLayouts.push('Kindle Fire/Nook H');
                }
                if (myDialog.layoutPanel.chkAndroid7V.value) {
                    requestedLayouts.push('Kindle Fire/Nook V');
                }
                if (myDialog.layoutPanel.chkPhoneH.value) {
                    requestedLayouts.push('Phone H');
                }
                if (myDialog.layoutPanel.chkPhoneV.value) {
                    requestedLayouts.push('Phone V');
                }
                try {
                    if (hasBook) {
                        TMAlternateLayouts.create(
                            app.activeBook,
                            requestedLayouts,
                            myDialog.optionsPanel.optionsLinkStories.value,
                            myDialog.optionsPanel.optionsCopyTextStyles.value
                        );
                    } else {
                        TMAlternateLayouts.createForDocument(
                            app.activeDocument,
                            requestedLayouts,
                            myDialog.optionsPanel.optionsLinkStories.value,
                            myDialog.optionsPanel.optionsCopyTextStyles.value
                        );
                    }
                } catch (myException) {
                    TMProgressBar.close();
                    TMDialogs.error(myException);
                }
                TMBook.resetCache();
            }
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs.showPreferences", myException);
        }
    },
    _setAlternateLayoutCheckbox: function (checkbox, hasBook, layoutName, layouts, articleCount) {
        try {
            var reportLayoutName = layoutName;
            if (reportLayoutName == 'Kindle Fire/Nook H') {
                reportLayoutName = 'Kindle Fire/Nook H (Android 7")';
            }
            if (reportLayoutName == 'Kindle Fire/Nook V') {
                reportLayoutName = 'Kindle Fire/Nook V (Android 7")';
            }
            if (hasBook) {
                var label = (articleCount == 1) ? 'article' : 'articles';
                if (layouts.hasOwnProperty(layoutName) && layouts[layoutName] == articleCount) {
                    checkbox.text = '"' + reportLayoutName + '" already exists in all articles';
                    checkbox.value = false;
                    checkbox.enabled = false;
                } else {
                    if (!layouts.hasOwnProperty(layoutName)) {
                        layouts[layoutName] = 0;
                    }
                    checkbox.text = 'Create "' + reportLayoutName + '" in ' + (articleCount - layouts[layoutName]) + ' of ' + articleCount + ' ' + label;
                    checkbox.value = true;
                    checkbox.enabled = true;
                }
            } else {
                if (!TMUtilities.itemInArray(layouts, layoutName)) {
                    checkbox.text = 'Create "' + reportLayoutName + '"';
                    checkbox.value = true;
                    checkbox.enabled = true;
                } else {
                    checkbox.text = '"' + reportLayoutName + '" already exists';
                    checkbox.value = false;
                    checkbox.enabled = false;
                }
            }
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs.showPreferences", myException);
        }
    },
    showPreferences: function () {
        try {
            var cacheSize = TMPreferences.readInt('exportCacheMaxSize', 5);
            var debugging = TMPreferences.isDebug();
            var warningsOnPreview = TMPreferences.readObject('preflightWarningsOnPreview', true);
            var warningsOnExport = TMPreferences.readObject('preflightWarningsOnExport', true);
            var enableLegacyOptions = TMPreferences.isRunningLegacyMode();
            var totalCacheSize = TMUtilities.formatFileSize(TMCache.totalSize());
            var myRes = "dialog { alignChildren: 'fill', text: '', \
cachingPanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Export Cache', \
cacheSize: Group { orientation: 'row', \
cacheSizeLabel: StaticText { text: 'Cache Size (GB)' }, \
cacheSizeText:  EditText { characters: 5, text: '', active: true }, \
cacheClearBtn:  Button { text:'Clear', properties: {name:'Clear'} }, \
}, \
cacheTotalSize: Group { orientation: 'row', \
cacheTotalSizeLabel: StaticText { text: 'Current Size' }, \
cacheTotalSizeText:  EditText { properties: { readonly: true } }, \
}, \
}, \
preflightPanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Preflight', \
preflightPreviewCheckbox: Checkbox { text: 'Show warnings during preview', value: true }, \
preflightExportCheckbox: Checkbox { text: 'Show warnings during sharing and exporting', value: true }, \
}, \
optionsPanel: Panel { orientation: 'column', alignChildren: 'left', text: 'Options', \
optionsLegacyModeCheckbox: Checkbox { text: 'Enable Legacy Options', value: false }, \
debuggingCheckbox: Checkbox { text: 'Enable Debugging Mode', value: true } \
}, \
buttons: Group { orientation: 'row', alignment: 'right', \
cancelBtn: Button { text:'Cancel', properties: {name:'cancel'} }, \
okBtn:     Button { text:'OK',     properties: {name:'ok'} }, \
} \
}";
            var myDialog = new Window(myRes, 'Preferences', undefined, {
                resizable: false,
                closeButton: false
            });
            myDialog.preferredSize.width = 340;
            myDialog.center();
            myDialog.cachingPanel.cacheSize.cacheSizeText.text = cacheSize;
            myDialog.cachingPanel.cacheTotalSize.cacheTotalSizeText.text = totalCacheSize;
            myDialog.optionsPanel.debuggingCheckbox.value = debugging;
            myDialog.preflightPanel.preflightPreviewCheckbox.value = warningsOnPreview;
            myDialog.preflightPanel.preflightExportCheckbox.value = warningsOnExport;
            myDialog.optionsPanel.optionsLegacyModeCheckbox.value = enableLegacyOptions;
            myDialog.cachingPanel.cacheSize.cacheSizeText.helpTip = 'To speed up the export, Twixl Publisher caches the exported articles. With this setting, you can define the maximum size of this cache. Setting this value to 0 disables the cache.';
            myDialog.optionsPanel.debuggingCheckbox.helpTip = 'Enabling debugging should only be turned on if requested by the support staff. Please note that caching is turned off when running in debugging mode.';
            myDialog.cachingPanel.cacheSize.cacheClearBtn.onClick = function () {
                if (!TMChecks.isHelperRunning()) {
                    return;
                }
                if (TMDialogs.confirm('Are you sure you want to delete the cache files?')) {
                    TMCache.clearCache();
                    var totalCacheSize = TMUtilities.formatFileSize(0);
                    myDialog.cachingPanel.cacheTotalSize.cacheTotalSizeText.text = totalCacheSize;
                }
            };
            TMUI.fixLabelWidth(myDialog.cachingPanel.cacheSize.cacheSizeLabel);
            TMUI.fixLabelWidth(myDialog.cachingPanel.cacheTotalSize.cacheTotalSizeLabel);
            TMUI.fixEditBoxWidth(myDialog.cachingPanel.cacheTotalSize.cacheTotalSizeText, 150);
            if (myDialog.show()) {
                TMPreferences.saveObject('exportCacheMaxSize', myDialog.cachingPanel.cacheSize.cacheSizeText.text);
                TMPreferences.saveObject('enableDebugging', myDialog.optionsPanel.debuggingCheckbox.value);
                TMPreferences.saveObject('preflightWarningsOnPreview', myDialog.preflightPanel.preflightPreviewCheckbox.value);
                TMPreferences.saveObject('preflightWarningsOnExport', myDialog.preflightPanel.preflightExportCheckbox.value);
                TMPreferences.saveObject('enableLegacyOptions', myDialog.optionsPanel.optionsLegacyModeCheckbox.value);
                TMPluginCore.isDebug = myDialog.optionsPanel.debuggingCheckbox.value;
                TMPreferences.dispatchPreferencesUpdated();
            }
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs.showPreferences", myException);
        }
    },
    showAbout: function () {
        try {
            var myRes = "dialog { alignChildren: 'fill', text: '', \
buttons: Group { orientation: 'column', alignment: 'center', \
codeName: StaticText { text: 'The \"Port Ellen\" Release' }, \
version: StaticText { text: 'Version 4.0 (12345)' }, \
copyRight: StaticText { text: '© 2010-" + new Date().getFullYear() + " Twixl media BVBA. All rights reserved.' }, \
okBtn:     Button { text:'OK', properties: {name:'ok'} } \
} \
}";
            var myDialog = new Window(myRes, 'Twixl Publisher', undefined, {
                resizable: false,
                closeButton: false
            });
            myDialog.preferredSize.width = 380;
            myDialog.buttons.codeName.text = 'The "' + TMVersion.codename + '" Release';
            myDialog.buttons.version.text = 'Version ' + TMVersion.version + ' (' + TMVersion.build + ')';
            TMUI.setFontColor(myDialog.buttons.copyRight, myDialog, [0.5, 0.5, 0.5]);
            myDialog.center();
            var myResult = myDialog.show();
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs.showAbout", myException);
        }
    },
    _getArticleTemplateNames: function (publicationStyle, showStatusBar) {
        try {
            var orientations = undefined;
            if (publicationStyle == 'Use Liquid and Alternate Layouts') {
                orientations = undefined;
            } else if (publicationStyle == 'Publication supports landscape only (legacy)') {
                orientations = 'landscape';
            } else if (publicationStyle == 'Publication supports portrait only (legacy)') {
                orientations = 'portrait';
            } else if (publicationStyle == 'Publication supports landscape and portrait (legacy)') {
                orientations = 'portrait+landscape';
            }
            return TMFiles.getTemplateNames(orientations, showStatusBar);
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs._getArticleTemplateNames", myException);
        }
    },
    _setupArticleTemplates: function (dropDownList, publicationStyle, showStatusBar) {
        try {
            var templates = TMDialogs._getArticleTemplateNames(publicationStyle, showStatusBar);
            var lastTemplate = TMPreferences.readObject(publicationStyle);
            dropDownList.removeAll();
            for (var i = 0; i < templates.length; i++) {
                var template = templates[i];
                var item = dropDownList.add('item', template);
                if (lastTemplate && template == lastTemplate) {
                    item.selected = true;
                }
            }
            if (!dropDownList.selection) {
                dropDownList.selection = dropDownList.items[0];
            }
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs._setupArticleTemplates", myException);
        }
    },
    _saveArticleTemplate: function (dropDownList, publicationStyle) {
        try {
            TMPreferences.saveObject(publicationStyle, dropDownList.selection.text);
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs._saveArticleTemplate", myException);
        }
    },
    _showTemplatesFolder: function (publicationStyle) {
        try {
            var templatesFolder = 'Templates';
            if (publicationStyle == 'Use Liquid and Alternate Layouts') {
                templatesFolder = 'Templates using Alternate Layout';
            }
            if (File.fs == 'Macintosh') {
                var myTemplateUserPath = '~/Library/Application Support/Twixl Publisher Plugin/' + templatesFolder;
            } else {
                var myTemplateUserPath = Folder.appData.fsName + '/Twixl Publisher/' + templatesFolder;
            }
            var myTemplateFolder = new Folder(myTemplateUserPath);
            myTemplateFolder.execute();
        } catch (myException) {
            TMStackTrace.addToStack("TMDialogs._showTemplatesFolder", myException);
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMDocument = {
    visibleMso: {},
    visibleScrollingContent: {},
    visibleImageSequences: {},
    visiblePanoramas: {},
    visibleHtmlItems: {},
    visibleButtons: {},
    hyperlinks: {},
    usesAlternateLayouts: {},
    linkedFiles: undefined,
};
Document.prototype.tmCustomThumbnailPath = function () {
    try {
        var articlePath = TMExporter.articlePath;
        articlePath = articlePath.replace('.indd', '');
        articlePath = articlePath.replace('.indt', '');
        articlePath = articlePath.replace('.idml', '');
        var thumbnailPath1 = new File(articlePath + '.jpg');
        if (thumbnailPath1.exists) {
            return thumbnailPath1;
        }
        var thumbnailPath2 = new File(articlePath + '@2x.jpg');
        if (thumbnailPath2.exists) {
            return thumbnailPath2;
        }
        return undefined;
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmCustomThumbnailPath", myException + ' (tm_document:39)');
        return undefined;
    }
};
Document.prototype.tmOrientation = function () {
    try {
        if (this.tmUsesAlternateLayouts()) {
            throw 'Not supported for alternate layouts';
        }
        var baseName = this.tmProperties()['originalBaseName'].replace('.indd', '').replace('.indt', '').replace('.idml', '');
        if (baseName.match("_Pt$") == '_Pt') {
            return 'portrait';
        } else {
            return 'landscape';
        }
    } catch (myException) {
        TMStackTrace.addToStack("Document.prototype.tmOrientation", myException);
    }
};
Document.prototype.tmDocumentIntent = function () {
    try {
        var myIntent = this.documentPreferences.intent;
        if (myIntent == DocumentIntentOptions.PRINT_INTENT) {
            return 'print';
        } else {
            return 'web'
        }
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmDocumentIntent", myException + ' (tm_document:69)');
        return 'web';
    }
}
Document.prototype.tmDocumentBaseResolution = function () {
    try {
        if (this.tmDocumentIntent() == 'print') {
            var currentUnit = app.scriptPreferences.measurementUnit;
            app.scriptPreferences.measurementUnit = MeasurementUnits.PIXELS;
            var pageWidth = this.documentPreferences.pageWidth;
            app.scriptPreferences.measurementUnit = currentUnit;
            if (pageWidth == 1024 || pageWidth == 1280 || pageWidth == 768 || pageWidth == 800 || pageWidth == 600) {
                return 72;
            } else {
                return 132;
            }
        } else {
            return 72;
        }
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmDocumentBaseResolution", myException + ' (tm_document:93)');
        return 72;
    }
};
Document.prototype.tmUsesAlternateLayouts = function () {
    try {
        if (TMPreferences.isRunningLegacyMode() == false) {
            return true;
        }
        var documentLayouts = TMAlternateLayouts.documentLayouts(this);
        if (documentLayouts.length > 1) {
            return true;
        }
        if (app.books.length > 0) {
            return app.activeBook.tmUsesAlternateLayouts();
        } else {
            return true; // Article-based workflow
        }
    } catch (myException) {
        TMLogger.silentException("Document.prototype.tmUsesAlternateLayouts", myException);
        return false;
    }
};
Document.prototype.tmShowStatusBar = function () {
    try {
        if (app.books.length == 0) {
            return true;
        }
        return app.activeBook.tmShowStatusBar();
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmShowStatusBar", myException + ' (tm_document:129)');
        return false;
    }
};
Document.prototype.tmShowInScrubber = function () {
    try {
        var myMetaData = this.tmProperties();
        return myMetaData['showInScrubber'] != false;
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmShowInScrubber", myException + ' (tm_document:139)');
        return '';
    }
};
Document.prototype.tmArticleTitle = function () {
    try {
        var myMetaData = this.tmProperties();
        if (myMetaData['title']) {
            return myMetaData['title'];
        }
        return '';
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmArticleTitle", myException + ' (tm_document:152)');
        return '';
    }
};
Document.prototype.tmArticleURL = function () {
    try {
        var myMetaData = this.tmProperties();
        if (myMetaData['articleURL']) {
            return myMetaData['articleURL'];
        }
        return '';
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmArticleURL", myException + ' (tm_document:165)');
        return '';
    }
};
Document.prototype.tmArticleTagLine = function () {
    try {
        var myMetaData = this.tmProperties();
        if (myMetaData['tagline']) {
            return myMetaData['tagline'];
        }
        return '';
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmArticleTagLine", myException + ' (tm_document:178)');
        return '';
    }
};
Document.prototype.tmArticleAuthor = function () {
    try {
        var myMetaData = this.tmProperties();
        if (myMetaData['author']) {
            return myMetaData['author'];
        }
        return '';
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmArticleAuthor", myException + ' (tm_document:191)');
        return '';
    }
};
Document.prototype.tmArticleName = function () {
    try {
        try {
            var baseName = this.tmProperties()['originalBaseName'];
            if (baseName) {
                return TMFiles.getBaseName(baseName, false);
            }
        } catch (myException) {
            TMLogger.error(myException);
        }
        return TMFiles.getBaseName(TMUtilities.decodeURI(this.name), false);
    } catch (myException) {
        TMStackTrace.addToStack("Document.prototype.tmArticleName", myException);
    }
};
Document.prototype.tmArticlePath = function (mySection) {
    try {
        var myPath = TMExporter.articlePath;
        if (this.tmUsesAlternateLayouts()) {
            myPath += '_' + mySection.tmLayoutDimensions();
            myPath += (mySection.tmOrientation() == 'landscape') ? '_Ls' : '_Pt';
        } else {
            myPath += (this.tmOrientation() == 'landscape') ? '_Ls' : '_Pt';
        }
        return myPath;
    } catch (myException) {
        TMStackTrace.addToStack("Document.prototype.tmArticlePath", myException);
    }
};
Document.prototype.tmAddProcessColor = function (myName, myColorValue) {
    try {
        try {
            myColor = this.colors.item(myName);
            myTest = myColor.name;
        } catch (myError) {
            myColor = this.colors.add();
        }
        myColor.properties = {
            name: myName,
            model: ColorModel.process,
            colorValue: myColorValue
        };
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmAddProcessColor", myException + ' (tm_document:241)');
    }
};
Document.prototype.tmPrepareForExport = function (isExport) {
    try {
        if (this.tmDocumentBaseResolution() == 132) {
            app.scriptPreferences.measurementUnit = MeasurementUnits.MILLIMETERS;
        } else {
            app.scriptPreferences.measurementUnit = MeasurementUnits.PIXELS;
        }
        TMLogger.debug('    Document Base Resolution is ' + this.tmDocumentBaseResolution() + ' dpi. Intent is ' + this.tmDocumentIntent());
        TMDocument.usesAlternateLayouts = {};
        TMPage.resetCache();
        if (isExport) {
            this.viewPreferences.rulerOrigin = RulerOrigin.PAGE_ORIGIN;
            TMDocument.linkedFiles = undefined;
            this.tmCacheLinkedFiles();
            this.tmCacheHyperlinks();
            this.tmSetExportColorSettings();
            this.tmResetZeroPoint();
            this.tmUnlockAllLayers();
            this.tmUngroupAllGroups();
        }
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmPrepareForExport", myException + ' (tm_document:272)');
    }
};
Document.prototype.tmCacheLinkedFiles = function () {
    try {
        TMDocument.linkedFiles = {};
        var myLinks = TMUtilities.collectionToArray(this.links);
        for (var i = 0; i < myLinks.length; i++) {
            var myLink = myLinks[i];
            if (myLink.hasOwnProperty('parent') && myLink.parent) {
                var myLinkFile = new File(myLink.filePath);
                TMDocument.linkedFiles[myLink.parent.id] = myLinkFile.fsName;
            }
        }
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmCacheLinkedFiles", myException + ' (tm_document:288)');
    }
};
Document.prototype.tmCacheHyperlinks = function () {
    try {
        TMDocument.hyperlinks = {};
        var myDocumentHyperlinks = TMUtilities.collectionToArray(this.hyperlinks);
        var myDocumentHyperlinksCount = myDocumentHyperlinks.length;
        for (var k = 0; k < myDocumentHyperlinksCount; k++) {
            var myHyperlink = myDocumentHyperlinks[k];
            if (!myHyperlink.isValid) {
                continue;
            }
            if (myHyperlink.source instanceof HyperlinkPageItemSource) {
                var myHyperlinkSource = myHyperlink.source.sourcePageItem;
                if (!myHyperlink.source.sourcePageItem.isValid) {
                    TMLogger.error('Invalid source page item: ' + myHyperlinkSource.toSource());
                    continue;
                }
                var myParentPage = TMPageItem.parentPage(myHyperlinkSource);
                if (!myParentPage) {
                    continue;
                }
                if (TMDocument.hyperlinks[myParentPage.id] == undefined) {
                    TMDocument.hyperlinks[myParentPage.id] = [];
                }
                if (myParentPage.parent.constructor.name == 'MasterSpread') {
                    var myDocumentPages = TMUtilities.collectionToArray(this.pages);
                    var myDocumentPagesCount = myDocumentPages.length;
                    for (var j = 0; j < myDocumentPagesCount; j++) {
                        var myPage = myDocumentPages[j];
                        if (myPage.appliedMaster != undefined && myParentPage.parent.id == myPage.appliedMaster.id) {
                            if (TMDocument.hyperlinks[myPage.id] == undefined) {
                                TMDocument.hyperlinks[myPage.id] = [];
                            }
                            TMDocument.hyperlinks[myPage.id].push(myHyperlink);
                        }
                    }
                } else {
                    TMDocument.hyperlinks[myParentPage.id].push(myHyperlink);
                }
            } else if (myHyperlink.source instanceof HyperlinkTextSource) {
                var myText = myHyperlink.source.sourceText;
                var myParentFrame = myText.parentTextFrames[0];
                var myParentPage = TMPageItem.parentPage(myParentFrame);
                if (!myParentPage) {
                    continue;
                }
                if (TMDocument.hyperlinks[myParentPage.id] == undefined) {
                    TMDocument.hyperlinks[myParentPage.id] = [];
                }
                TMDocument.hyperlinks[myParentPage.id].push(myHyperlink);
            }
        }
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmCacheHyperlinks", myException + ' (tm_document:360)');
    }
};
Document.prototype.tmUnlockAllLayers = function () {
    try {
        this.layers.everyItem().locked = false;
        var docLayers = TMUtilities.collectionToArray(this.layers);
        var cnt = docLayers.length;
        for (var i = 0; i < cnt; i++) {
            var layer = docLayers[i];
            if (layer.locked) {
                layer.locked = false;
            }
            if (!layer.visible || !layer.printable) {
                layer.remove();
            }
        };
    } catch (myException) {
        TMLogger.silentException("Document.prototype.tmUnlockAllLayers", myException);
    }
};
Document.prototype.tmUngroupAllGroups = function () {
    try {
        if (this.groups.everyItem().length > 0) {
            TMLogger.info("Ungrouping all items in the document");
        }
        this.groups.everyItem().ungroup();
    } catch (myException) { }
};
Document.prototype.tmSetExportColorSettings = function () {
    try {
        this.cmykPolicy = ColorSettingsPolicy.PRESERVE_EMBEDDED_PROFILES;
        this.rgbPolicy = ColorSettingsPolicy.PRESERVE_EMBEDDED_PROFILES;
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmSetExportColorSettings", myException + ' (tm_document:399)');
    }
};
Document.prototype.tmResetZeroPoint = function () {
    try {
        this.zeroPoint = [0, 0];
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmStoreZeroPoint", myException + ' (tm_document:407)');
    }
};
Document.prototype.resetAllMultiStateObjects = function () { };
Document.prototype.tmHideInteractiveContents = function () {
    try {
        TMDocument.visibleMso = {};
        TMDocument.visibleScrollingContent = {};
        TMDocument.visibleImageSequences = {};
        TMDocument.visiblePanoramas = {};
        TMDocument.visibleHtmlItems = {};
        TMDocument.visibleButtons = {};
        var myDocMso = TMUtilities.collectionToArray(this.multiStateObjects);
        var myDocMsoCount = myDocMso.length;
        for (var m = 0; m < myDocMsoCount; m++) {
            var mySlideShow = myDocMso[m];
            if (mySlideShow.visible) {
                mySlideShow.visible = false;
                TMDocument.visibleMso[mySlideShow.id] = mySlideShow.id;
            }
        }
        var myDocButtons = TMUtilities.collectionToArray(this.buttons);
        var myDocButtonCount = myDocButtons.length;
        for (var m = 0; m < myDocButtonCount; m++) {
            var myButton = myDocButtons[m];
            var myStates = TMUtilities.collectionToArray(myButton.states);
            if (myButton.visible && myStates.length > 1) {
                var myStateNames = [];
                for (var i = 0; i < myStates.length; i++) {
                    myStateNames.push(myStates[i].statetype);
                }
                if (!TMUtilities.itemInArray(myStateNames, StateTypes.DOWN)) {
                    continue;
                }
                myButton.visible = false;
                TMDocument.visibleButtons[myButton.id] = myButton.id;
            }
        }
        var myDocItems = this.allPageItems.slice(0);
        var myDocItemCount = myDocItems.length;
        for (var m = 0; m < myDocItemCount; m++) {
            var myPageItem = myDocItems[m];
            var myLabel = myPageItem.extractLabel("com.rovingbird.epublisher.sc");
            if (myLabel != '') {
                var properties = TMJSON.parse(myLabel);
                if (properties.scAllowScrolling && myPageItem.visible && myPageItem.allPageItems.length > 0) {
                    myPageItem.visible = false;
                    TMDocument.visibleScrollingContent[myPageItem.id] = myPageItem.id;
                }
            }
            var myLabel = myPageItem.extractLabel("com.rovingbird.epublisher.imagesequence");
            if (myLabel != '') {
                if (myPageItem.visible) {
                    myPageItem.visible = false;
                    TMDocument.visibleImageSequences[myPageItem.id] = myPageItem.id;
                }
            }
            var myLabel = myPageItem.extractLabel("com.rovingbird.epublisher.panorama");
            if (myLabel != '') {
                if (myPageItem.visible) {
                    myPageItem.visible = false;
                    TMDocument.visiblePanoramas[myPageItem.id] = myPageItem.id;
                }
            }
            if (myPageItem.constructor.name == 'HtmlItem') {
                if (myPageItem.parent && myPageItem.parent.visible) {
                    myPageItem.visible = false;
                    TMDocument.visibleHtmlItems[myPageItem.id] = myPageItem.id;
                }
            }
        }
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmHideInteractiveContents", myException + ' (tm_document:504)');
    }
};
Document.prototype.tmRestoreInteractiveContents = function () {
    try {
        var myDocMso = TMUtilities.collectionToArray(this.multiStateObjects);
        var myDocMsoCount = myDocMso.length;
        for (var m = 0; m < myDocMsoCount; m++) {
            var mySlideShow = myDocMso[m];
            if (TMDocument.visibleMso[mySlideShow.id]) {
                mySlideShow.visible = true;
            }
        }
        var myDocButtons = TMUtilities.collectionToArray(this.buttons);
        var myDocButtonCount = myDocButtons.length;
        for (var m = 0; m < myDocButtonCount; m++) {
            var myButton = myDocButtons[m];
            if (TMDocument.visibleButtons[myButton.id]) {
                myButton.visible = true;
            }
        }
        var myDocItems = this.allPageItems.slice(0);
        var myDocItemCount = myDocItems.length;
        for (var m = 0; m < myDocItemCount; m++) {
            var myPageItem = myDocItems[m];
            if (TMDocument.visibleScrollingContent[myPageItem.id]) {
                myPageItem.visible = true;
            }
            if (TMDocument.visibleImageSequences[myPageItem.id]) {
                myPageItem.visible = true;
            }
            if (TMDocument.visiblePanoramas[myPageItem.id]) {
                myPageItem.visible = true;
            }
            if (TMDocument.visibleHtmlItems[myPageItem.id]) {
                myPageItem.visible = true;
            }
        }
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmRestoreInteractiveContents", myException + ' (tm_document:551)');
    }
};
Document.prototype.tmHyperlinks = function (myPage, myParent) {
    try {
        var myDocumentHyperlinks = TMDocument.hyperlinks[myPage.id];
        if (myDocumentHyperlinks == undefined) {
            return [];
        }
        var myHyperlinks = [];
        var myDocumentHyperlinksCount = myDocumentHyperlinks.length;
        for (var k = 0; k < myDocumentHyperlinksCount; k++) {
            try {
                var myHyperlink = myDocumentHyperlinks[k];
                var myBounds = undefined;
                if (myHyperlink.source instanceof HyperlinkPageItemSource) {
                    var myHyperlinkSource = myHyperlink.source.sourcePageItem;
                    if (myParent != undefined && TMPageItem.isChildOf(myHyperlinkSource, myParent) == false) {
                        continue;
                    }
                    if (myParent == undefined && TMPageItem.isNested(myHyperlinkSource)) {
                        continue;
                    }
                    myBounds = [TMGeometry.getBounds(myHyperlinkSource.visibleBounds)];
                } else {
                    var myText = myHyperlink.source.sourceText;
                    var myParentFrame = myText.parentTextFrames[0];
                    if (myParent != undefined && TMPageItem.isChildOf(myParentFrame, myParent) == false) {
                        continue;
                    }
                    if (myParent == undefined && TMPageItem.isNested(myParentFrame)) {
                        continue;
                    }
                    myBounds = TMGeometry.getTextBounds(myText);
                }
                if (myBounds != undefined) {
                    var myLinkType = undefined;
                    var myLinkData = undefined;
                    try {
                        var myDestination = myHyperlink.destination;
                    } catch (myException) {
                        continue;
                    }
                    if (myHyperlink.destination instanceof HyperlinkURLDestination) {
                        var url = TMUtilities.normalizeUrl(myHyperlink.destination.destinationURL);
                        if (url != undefined) {
                            myLinkType = "weblinks";
                            myLinkData = {
                                id: myHyperlink.id,
                                url: url.tmTrim(),
                            };
                        }
                    }
                    if (myHyperlink.destination instanceof HyperlinkPageDestination) {
                        myLinkType = "pagelinks";
                        if (myHyperlink.destination.destinationPage && myHyperlink.parent) {
                            myLinkData = {
                                id: myHyperlink.id,
                                page: myHyperlink.destination.destinationPage.name - myHyperlink.parent.pages.firstItem().name + 1,
                                article: this.tmArticleName(),
                            };
                        }
                    }
                    if (myHyperlink.destination instanceof HyperlinkExternalPageDestination) {
                        var myDestination = myHyperlink.destination;
                        var myDestinationArticle = myDestination.documentPath.tmArticleName();
                        if (myDestinationArticle != undefined && myDestinationArticle != "") {
                            myLinkType = "pagelinks";
                            myLinkData = {
                                id: myHyperlink.id,
                                page: myDestination.destinationPageIndex,
                                article: myDestination.documentPath.tmArticleName(),
                            };
                        }
                    }
                    if (myLinkType != undefined && myLinkData != undefined) {
                        for (var l in myBounds) {
                            try {
                                var myBound = myBounds[l];
                                myLinkData = TMGeometry.mergeBounds(myLinkData, myBound);
                                delete myBounds['pageId'];
                                myLinkData['linkType'] = myLinkType;
                                myHyperlinks.push(myLinkData);
                            } catch (myException) {
                                TMLogger.error('Failed to list hyperlinks: ' + myException);
                            }
                        }
                    }
                }
            } catch (myException) {
                TMLogger.exception("Document.prototype.tmHyperlinks", myException + ' (tm_document:657)');
            }
        }
        return myHyperlinks;
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmHyperlinks", myException + ' (tm_document:665)');
        return [];
    }
};
Document.prototype.tmProperties = function () {
    try {
        var myMetaData = undefined;
        if (this.label != undefined && this.label != '') {
            try {
                myMetaData = eval(this.label.replace(/\n+/g, ''));
            } catch (myException) {
                TMLogger.silentException("Document.prototype.tmProperties", myException);
            }
        }
        if (!myMetaData) {
            myMetaData = {
                title: this.metadataPreferences.documentTitle
            };
        }
        return myMetaData;
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmProperties", myException + ' (tm_document:685)');
        return {};
    }
};
Document.prototype.tmIsArticle = function () {
    try {
        var myMetaData = {};
        try {
            myMetaData = eval(this.label);
        } catch (myException) { }
        return (myMetaData != undefined && myMetaData['title'] != undefined);
    } catch (myException) {
        TMLogger.exception("Document.prototype.tmIsArticle", myException + ' (tm_document:699)');
        return false;
    }
};
Document.prototype.tmChecksum = function () {
    try {
        return kEXPORT_FORMAT + '_' + TMExporter.documentChecksum;
    } catch (myException) {
        TMLogger.error('Failed to generate checksum: ' + this.filePath.fsName + '/' + this.name);
        TMLogger.exception("Document.prototype.tmChecksum", myException + ' (tm_document:709)');
        return undefined;
    }
};
Document.prototype.tmCopyBackgroundMusic = function (exportFolder) {
    var properties = this.tmProperties();
    if (!properties.hasOwnProperty('backgroundMusicPlaylist')) {
        return undefined;
    }
    var backgroundMusicPlaylist = properties['backgroundMusicPlaylist'];
    if (!backgroundMusicPlaylist || backgroundMusicPlaylist == '') {
        return undefined;
    }
    backgroundMusicPlaylist = TMFiles.pathRelToAbs(this.filePath.fsName, backgroundMusicPlaylist);
    var backgroundMusicPlaylistFolder = new Folder(backgroundMusicPlaylist);
    if (!backgroundMusicPlaylistFolder.exists) {
        TMLogger.info('    Background playlist folder "' + backgroundMusicPlaylist + '" does not exist.');
        return undefined;
    }
    var filesToCopy = backgroundMusicPlaylistFolder.getFiles('*.mp3');
    var totalSize = 0;
    for (var i = 0; i < filesToCopy.length; i++) {
        totalSize += filesToCopy[i].length;
    }
    if (filesToCopy.length == 0 || totalSize == 0) {
        TMLogger.info('    Background playlist folder "' + backgroundMusicPlaylist + '" does not contain any files.');
        return undefined;
    }
    totalSize = TMUtilities.formatFileSize(totalSize, 0);
    var playlistPath = TMHelper.call('fs/realpath', {
        path: backgroundMusicPlaylistFolder.fsName
    }).path
    TMLogger.info("    Background music playlist path: " + playlistPath);
    var playlistName = TMCrypto.MD5(playlistPath);
    TMLogger.info("    Background music Playlist ID: " + playlistName);
    if (TMCache.isMediaLinkCached(backgroundMusicPlaylistFolder.fsName)) {
        TMLogger.info("    Cached the background music (" + totalSize + ", " + filesToCopy.length + " files)");
        return playlistName;
    }
    var myMedia = new Folder(exportFolder.fsName + "/MediaResources/BackgroundMusic/" + playlistName);
    if (!myMedia.exists) {
        TMLogger.debug('    Creating: ' + myMedia.fsName);
        myMedia.create();
    }
    TMLogger.info("    Copying the background music (" + totalSize + ", " + filesToCopy.length + " files)");
    for (var i = 0; i < filesToCopy.length; i++) {
        var file = filesToCopy[i];
        TMLogger.debug('Copying: ' + file.fsName);
        TMLogger.debug('     To: ' + myMedia.fsName + '/' + file.displayName);
        file.copy(myMedia.fsName + '/' + file.displayName);
    }
    TMCache.cacheMediaLink(backgroundMusicPlaylistFolder.fsName, playlistName);
    return playlistName;
}
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMEventHandlers = {
    EVENT_LISTENER_AFTER_SELECTION_CHANGED: 'tp-after-selection-changed',
    EVENT_LISTENER_AFTER_SELECTION_ATTR_CHANGED: 'tp-after-selection-attr-changed',
    KEY_MSO: "com.rovingbird.epublisher.mso",
    KEY_WV: "com.rovingbird.epublisher.wv",
    KEY_WO: "com.rovingbird.epublisher.wo",
    KEY_SC: "com.rovingbird.epublisher.sc",
    KEY_IMAGE: "com.rovingbird.epublisher.image",
    KEY_MOVIE: "com.rovingbird.epublisher.movie",
    KEY_SOUND: "com.rovingbird.epublisher.sound",
    KEY_IMAGESEQUENCE: "com.rovingbird.epublisher.imagesequence",
    KEY_PANORAMA: "com.rovingbird.epublisher.panorama",
    KEY_WIDGET: "com.rovingbird.epublisher.widget",
    xlib: undefined,
    init: function () {
        if (TMPluginCore.engine != 'html') {
            return;
        }
        TMLogger.debug('Registering event handlers');
        TMLogger.debug('App has ' + app.eventListeners.length + ' event listener(s)');
        TMEventHandlers.xlib = new ExternalObject("lib:\PlugPlugExternalObject");
        this.removeEventListener();
        this.registerEventListener();
        this.afterSelectionChanged(null);
    },
    registerEventListener: function () {
        if (TMPluginCore.engine != 'html') {
            return;
        }
        TMLogger.debug('Registering: ' + this.EVENT_LISTENER_AFTER_SELECTION_CHANGED);
        var eventListener = app.addEventListener("afterSelectionChanged", TMEventHandlers.afterSelectionChanged, false);
        eventListener.name = this.EVENT_LISTENER_AFTER_SELECTION_CHANGED;
        TMLogger.debug('Registered event listener: ' + this.EVENT_LISTENER_AFTER_SELECTION_CHANGED);
        TMLogger.debug('Registering: ' + this.EVENT_LISTENER_AFTER_SELECTION_ATTR_CHANGED);
        var eventListener = app.addEventListener("afterSelectionAttributeChanged", TMEventHandlers.afterSelectionChanged, false);
        eventListener.name = this.EVENT_LISTENER_AFTER_SELECTION_ATTR_CHANGED;
        TMLogger.debug('Registered event listener: ' + this.EVENT_LISTENER_AFTER_SELECTION_ATTR_CHANGED);
    },
    removeEventListener: function () {
        if (TMPluginCore.engine != 'html') {
            return;
        }
        for (var i = 0; i < app.eventListeners.length; i++) {
            var eventListener = app.eventListeners[i];
            if (eventListener.name == this.EVENT_LISTENER_AFTER_SELECTION_CHANGED || eventListener.name == this.EVENT_LISTENER_AFTER_SELECTION_ATTR_CHANGED) {
                TMLogger.debug('Removed event listener: ' + eventListener.name);
                eventListener.remove();
            } else {
                TMLogger.debug('EL: ' + eventListener.eventType + ' (name: ' + eventListener.name + ')');
            }
        };
    },
    afterSelectionChanged: function (event) {
        if (TMPluginCore.engine != 'html') {
            return;
        }
        try {
            var selection = TMApplication.appSelection();
            if (selection) {
                var clazz = selection.constructor.name;
                TMLogger.debug('afterSelectionChanged: selected class: ' + clazz);
                var objectScrollable = TMEventHandlers.loadLabel(selection, TMEventHandlers.KEY_SC);
                var objectWebOverlay = TMEventHandlers.loadLabel(selection, TMEventHandlers.KEY_WO);
                var objectWebViewer = TMEventHandlers.loadLabel(selection, TMEventHandlers.KEY_WV);
                var objectImgSequence = TMEventHandlers.loadLabel(selection, TMEventHandlers.KEY_IMAGESEQUENCE);
                var objectPanorama = TMEventHandlers.loadLabel(selection, TMEventHandlers.KEY_PANORAMA);
                var objectWidget = TMEventHandlers.loadLabel(selection, TMEventHandlers.KEY_WIDGET);
                var objectImage = TMEventHandlers.loadLabel(selection, TMEventHandlers.KEY_IMAGE);
                var objectMovie = TMEventHandlers.loadLabel(selection, TMEventHandlers.KEY_MOVIE);
                var objectSound = TMEventHandlers.loadLabel(selection, TMEventHandlers.KEY_SOUND);
                var objectSlideShow = TMEventHandlers.loadLabel(selection, TMEventHandlers.KEY_MSO);
                if (objectScrollable) {
                    return TMEventHandlers.eventSelectionChanged("scrollable_content", objectScrollable, selection);
                }
                if (objectWebOverlay) {
                    return TMEventHandlers.eventSelectionChanged("web_overlay", objectWebOverlay, selection);
                }
                if (objectWebViewer) {
                    return TMEventHandlers.eventSelectionChanged("web_viewer", objectWebViewer, selection);
                }
                if (objectImgSequence) {
                    return TMEventHandlers.eventSelectionChanged("image_sequence", objectImgSequence, selection);
                }
                if (objectImage) {
                    return TMEventHandlers.eventSelectionChanged("image", objectImage, selection);
                }
                if (objectPanorama) {
                    return TMEventHandlers.eventSelectionChanged("panorama", objectPanorama, selection);
                }
                if (objectWidget) {
                    if (objectWidget.widget) {
                        var widgetType = objectWidget.widget.split('.').pop();
                        return TMEventHandlers.eventSelectionChanged('widget_' + widgetType, objectWidget, selection);
                    } else {
                        return TMEventHandlers.eventSelectionChanged("assign_element", 'rectangle');
                    }
                }
                if (clazz == 'Movie') {
                    return TMEventHandlers.eventSelectionChanged("movie", objectMovie, selection);
                }
                if (clazz == 'Sound') {
                    return TMEventHandlers.eventSelectionChanged("sound", objectSound, selection);
                }
                if (clazz == 'MultiStateObject') {
                    return TMEventHandlers.eventSelectionChanged("slideshow", objectSlideShow, selection);
                }
                if (clazz == 'TextFrame') {
                    if (objectWebOverlay) {
                        return TMEventHandlers.eventSelectionChanged("web_overlay", objectWebOverlay, selection);
                    } else {
                        return TMEventHandlers.eventSelectionChanged("assign_element", undefined, undefined, 'textframe');
                    }
                }
                if (clazz == 'Graphic' || clazz == 'EPS' || clazz == 'Image' || clazz == 'PDF' || clazz == 'PICT' || clazz == 'WMF' || clazz == 'ImportedPage') {
                    if (objectImgSequence) {
                        return TMEventHandlers.eventSelectionChanged("image_sequence", objectImgSequence, selection);
                    }
                    if (objectWebOverlay) {
                        return TMEventHandlers.eventSelectionChanged("web_overlay", objectWebOverlay, selection);
                    }
                    if (objectImage) {
                        return TMEventHandlers.eventSelectionChanged("image", objectImage, selection);
                    }
                    return TMEventHandlers.eventSelectionChanged("assign_element", undefined, undefined, 'image');
                }
                if (clazz == 'Rectangle') {
                    if (selection.sounds.length == 0 && selection.movies.length == 0 && selection.allPageItems.length == 0) {
                        if (objectWebOverlay) {
                            return TMEventHandlers.eventSelectionChanged("web_overlay", objectWebOverlay, selection);
                        }
                        if (objectWebViewer) {
                            return TMEventHandlers.eventSelectionChanged("web_viewer", objectWebViewer, selection);
                        }
                        if (objectImgSequence) {
                            return TMEventHandlers.eventSelectionChanged("image_sequence", objectImgSequence, selection);
                        }
                        if (objectPanorama) {
                            return TMEventHandlers.eventSelectionChanged("panorama", objectPanorama, selection);
                        }
                        if (objectWidget && objectWidget.widget) {
                            var widgetType = objectWidget.widget.split('.').pop();
                            return TMEventHandlers.eventSelectionChanged('widget_' + widgetType, objectWidget, selection);
                        }
                        return TMEventHandlers.eventSelectionChanged("assign_element", undefined, undefined, 'rectangle');
                    }
                    if (selection.allPageItems.length > 0 && selection.sounds.length == 0 && selection.movies.length == 0 && selection.graphics.length == 0 && selection.htmlItems.length == 0) {
                        return TMEventHandlers.eventSelectionChanged("scrollable_content", objectScrollable, selection);
                    }
                    if (selection.graphics.length == 1) {
                        selection = selection.graphics[0];
                        var objectWebOverlay = TMEventHandlers.loadLabel(selection, TMEventHandlers.KEY_WO);
                        var objectImgSequence = TMEventHandlers.loadLabel(selection, TMEventHandlers.KEY_IMAGESEQUENCE);
                        var objectPanorama = TMEventHandlers.loadLabel(selection, TMEventHandlers.KEY_PANORAMA);
                        var objectImage = TMEventHandlers.loadLabel(selection, TMEventHandlers.KEY_IMAGE);
                        if (objectImgSequence) {
                            return TMEventHandlers.eventSelectionChanged("image_sequence", objectImgSequence, selection);
                        }
                        if (objectPanorama) {
                            return TMEventHandlers.eventSelectionChanged("panorama", objectPanorama, selection);
                        }
                        if (objectWebOverlay) {
                            return TMEventHandlers.eventSelectionChanged("web_overlay", objectWebOverlay);
                        }
                        if (objectImage) {
                            return TMEventHandlers.eventSelectionChanged("image", objectImage, selection);
                        }
                        return TMEventHandlers.eventSelectionChanged("assign_element", undefined, undefined, 'image');
                    }
                    if (selection.movies.length == 1) {
                        objectMovie = TMEventHandlers.loadLabel(selection.movies[0], TMEventHandlers.KEY_MOVIE);
                        return TMEventHandlers.eventSelectionChanged("movie", objectMovie, selection.movies[0]);
                    }
                    if (selection.sounds.length == 1) {
                        objectSound = TMEventHandlers.loadLabel(selection.sounds[0], TMEventHandlers.KEY_SOUND);
                        return TMEventHandlers.eventSelectionChanged("sound", objectSound, selection.sounds[0]);
                    }
                }
            }
            TMEventHandlers.eventSelectionChanged("home");
        } catch (e) {
            TMLogger.silentException('TMEventHandlers.afterSelectionChanged', e + ' - ' + e.line);
            TMEventHandlers.eventSelectionChanged("home");
        }
    },
    objectHasProperty: function (object, property) {
        if (!object || !object[property] || object[property] == '') {
            return false;
        } else {
            return true;
        }
    },
    eventSelectionChanged: function (panel, objectData, object, type) {
        try {
            if (TMPluginCore.engine != 'html') {
                return;
            }
            TMLogger.debug('Selection changed: ' + panel);
            if (objectData == undefined) {
                objectData = {};
            }
            if (panel.tmStartsWith('widget_')) {
                if (objectData.hasOwnProperty('properties')) {
                    objectData = TMUtilities.mergeArrays(objectData, objectData['properties']);
                    delete objectData['properties'];
                }
            }
            if (panel == 'sound') {
                if (!this.objectHasProperty(objectData, 'soundAnalyticsName')) {
                    objectData['soundAnalyticsName'] = object.id;
                }
                objectData['soundAutoStart'] = object.playOnPageTurn;
                objectData['soundLoop'] = object.soundLoop;
            } else if (panel == 'movie') {
                if (!this.objectHasProperty(objectData, 'movieAnalyticsName')) {
                    objectData['movieAnalyticsName'] = object.id;
                }
                objectData['movieAutoStart'] = object.playOnPageTurn;
                objectData['movieLoop'] = object.movieLoop;
                objectData['movieController'] = (object.controllerSkin != 'SkinOverAll');
            } else if (panel == 'scrollable_content') {
                if (!this.objectHasProperty(objectData, 'scAnalyticsName')) {
                    objectData['scAnalyticsName'] = object.id;
                }
            } else if (panel == 'web_overlay') {
                if (!this.objectHasProperty(objectData, 'woAnalyticsName')) {
                    objectData['woAnalyticsName'] = object.id;
                }
            } else if (panel == 'image') {
                if (!this.objectHasProperty(objectData, 'imageAnalyticsName')) {
                    objectData['imageAnalyticsName'] = object.id;
                }
            } else if (panel == 'slideshow') {
                if (!this.objectHasProperty(objectData, 'msoAnalyticsName')) {
                    objectData['msoAnalyticsName'] = object.name;
                }
                if (!this.objectHasProperty(objectData, 'msoTransitionStyle')) {
                    objectData['msoTransitionStyle'] = 'None';
                }
                if (!this.objectHasProperty(objectData, 'msoTransitionDuration')) {
                    objectData['msoTransitionDuration'] = '1';
                }
            } else {
                if (object && object.hasOwnProperty('id') && !this.objectHasProperty(objectData, 'analyticsName')) {
                    objectData['analyticsName'] = object.id;
                }
            }
            TMLogger.debug('Object: ' + object);
            if (objectData) {
                TMLogger.debug('Object Data: ' + TMJSON.stringify(objectData));
            }
            TMUtilities.dispatchEvent(
                "com.twixlmedia.publisher.indesign.events.afterSelectionChanged", {
                    "panel": panel,
                    "data": objectData,
                    'type': type
                }
            );
        } catch (e) {
            TMLogger.silentException('TMEventHandlers.eventSelectionChanged', e + ' - ' + e.line);
        }
    },
    loadLabel: function (object, labelKey) {
        if (!object.hasOwnProperty('extractLabel')) {
            return undefined;
        }
        var labelString = object.extractLabel(labelKey);
        if (!labelString) {
            return undefined;
        }
        TMLogger.debug('Loaded label ' + labelKey + ': ' + labelString);
        var labelProperties = TMJSON.parse(labelString);
        return labelProperties;
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMExporter = {
    savedBookAutomaticPagination: undefined,
    savedBaselineStart: undefined,
    savedBaselineRelative: undefined,
    exportDeviceType: undefined,
    exportPublicationLayouts: undefined,
    exportPublicationLayout: [],
    exportID: undefined, // Used for the widgets
    documentChecksum: undefined,
    articleName: undefined,
    articlePath: undefined,
    articleParentPath: undefined,
    emptyPngName: undefined,
    supportedLayouts: ['ipad h', 'ipad v', 'android 10" h', 'android 10" v', 'kindle fire/nook h', 'kindle fire/nook v', 'phone h', 'phone v'],
    reset: function () {
        this.exportDeviceType = undefined;
        this.exportPublicationLayouts = undefined;
        this.exportPublicationLayout = [];
    },
    prepareForExport: function (type) {
        try {
            TMLogger.info("Setting up new export process: " + type);
            TMExporter.savedBookAutomaticPagination = undefined;
            if (app.books.length > 0 && app.activeBook) {
                TMExporter.savedBookAutomaticPagination = app.activeBook.automaticPagination;
            }
            TMLogger.info('Saved automaticPagination: ' + TMExporter.savedBookAutomaticPagination);
        } catch (myException) {
            TMLogger.exception("TMExporter.prepareForExport", myException + ' (tm_exporter:47)');
        }
    },
    finishExport: function (type) {
        try {
            TMLogger.info("Finishing export process: " + type);
            if (app.books.length > 0 && app.activeBook && TMExporter.savedBookAutomaticPagination != undefined) {
                app.activeBook.automaticPagination = TMExporter.savedBookAutomaticPagination;
                TMLogger.info('Restored automaticPagination: ' + TMExporter.savedBookAutomaticPagination);
            }
        } catch (myException) {
            TMLogger.exception("TMExporter.finishExport", myException + ' (tm_exporter:62)');
        }
    },
    exportPublication: function (myBook, myRootFolderName, myOptions) {
        TMTimer.start("export", "Starting new publication export to: " + myRootFolderName);
        TMLogger.info('Export format: ' + kEXPORT_FORMAT);
        this.prepareForExport('exportPublication');
        TMBook.resetCache();
        TMCache.reset();
        this.savedBaselineStart = undefined;
        this.savedBaselineRelative = undefined;
        var myDeviceType = myOptions['previewDeviceType'];
        if (myDeviceType) {
            TMLogger.info('Exporting for device type: ' + myDeviceType);
            this.exportDeviceType = myDeviceType;
        }
        var myProgressTitle = "Export Publication";
        if (myOptions['article']) {
            myProgressTitle = "Export Article";
            TMLogger.info('Exporting single article: ' + myOptions['article']);
        }
        var scrubber = myBook.tmShowScrubber();
        var articlesInScrubber = false;
        var myData = {};
        TMProgressBar.create(myProgressTitle, 'Preparing the export...', 1);
        var myArticleCount = myBook.tmFileCount();
        var myWebResourcesSteps = 1;
        var myPublicationXmlSteps = 2;
        var myTotalProgressSteps = myArticleCount + myWebResourcesSteps + myPublicationXmlSteps;
        TMProgressBar.create(myProgressTitle, 'Exporting publication...', myTotalProgressSteps);
        try {
            if (myDeviceType) {
                this.exportPublicationLayout = [];
                if (myBook.tmUsesAlternateLayouts()) {
                    this.determineExportLayouts(myDeviceType);
                } else {
                    if (myDeviceType.tmStartsWith('phone') == false) {
                        this.exportPublicationLayout = ['ipad h', 'ipad v'];
                    }
                }
                TMLogger.info("Preview Device Type: " + myDeviceType);
                TMLogger.info("Publication layouts: " + this.exportPublicationLayouts.toSource());
                TMLogger.info("Exporting layouts: " + this.exportPublicationLayout.toSource());
                if (this.exportPublicationLayout.length == 0) {
                    TMProgressBar.close();
                    TMLogger.error("Found no compatible layouts to preview.");
                    TMDialogs.error("Found no compatible layouts to preview.");
                    this.finishExport('exportPublication');
                    return;
                }
            }
            var myDstPublication = new Folder(myRootFolderName);
            var myTmpPublication = new Folder(Folder.temp.fsName + '/' + kTMP_PREFIX + TMUtilities.uuid());
            myTmpPublication.create();
            TMLogger.debug('Created temp publication: ' + myTmpPublication.fsName);
            this.prepareMediaResources(myTmpPublication);
            this.emptyPngName = this.exportWebResources(myBook.filePath.fsName, myTmpPublication);
            TMProgressBar.updateLabel("Exporting: " + TMUtilities.decodeURI(myBook.name));
            TMProgressBar.updateProgressBySteps(myWebResourcesSteps);
            var myBookContents = TMUtilities.collectionToArray(myBook.bookContents);
            var myBookContentsCount = myBookContents.length;
            for (var i = 0; i < myBookContentsCount; i++) {
                var myFile = myBookContents[i];
                if (myOptions['article'] && (myFile.tmArticleName() != myOptions['article'])) {
                    TMProgressBar.updateProgressBySteps(1);
                    continue;
                }
                TMProgressBar.updateLabel(TMUtilities.decodeURI(myFile.fullName.name));
                if (!myFile.fullName.exists) {
                    TMLogger.error("Skipping file: " + myFile.fullName.name + " (file doesn't exist)");
                    TMProgressBar.updateProgressBySteps(1);
                    continue;
                }
                TMProgressBar.updateSubLabel("Opening: " + TMUtilities.decodeURI(myFile.fullName.name));
                TMLogger.info("Opening: " + TMUtilities.decodeURI(myFile.fullName.fsName));
                var mySrcArticleFile = myFile.fullName;
                TMExporter.documentChecksum = TMHelper.call('fs/checksum', {
                    path: myFile.fullName.fsName
                }).checksum
                TMExporter.articleName = TMUtilities.decodeURI(mySrcArticleFile.name);
                TMExporter.articlePath = mySrcArticleFile.fsName;
                TMExporter.articleParentPath = mySrcArticleFile.parent.fsName;
                if (myDocument = app.open(mySrcArticleFile, false, OpenOptions.OPEN_COPY)) {
                    var documentProperties = myDocument.tmProperties();
                    documentProperties['originalFileName'] = myFile.fullName;
                    documentProperties['originalBaseName'] = TMUtilities.decodeURI(myFile.name);
                    myDocument.label = documentProperties.toSource();
                    app.scriptPreferences.enableRedraw = false;
                    TMPageItem.tmClearCache();
                    myDocument.tmPrepareForExport(true);
                    TMLogger.info("  Checksum: " + myDocument.tmChecksum());
                    var backgroundMusicPlaylist = myDocument.tmCopyBackgroundMusic(myTmpPublication);
                    var myPageNumber = 0;
                    gBaseResolution = myDocument.tmDocumentBaseResolution();
                    var myArticleName = myDocument.tmArticleName();
                    var myArticleTitle = myDocument.tmArticleTitle();
                    var myArticleTagLine = myDocument.tmArticleTagLine();
                    var myArticleAuthor = myDocument.tmArticleAuthor();
                    var myArticleURL = myDocument.tmArticleURL();
                    var myArticleInScrubber = scrubber ? myDocument.tmShowInScrubber() : false;
                    if (myArticleInScrubber == 'yes' || myArticleInScrubber === true) {
                        articlesInScrubber = true;
                    }
                    var myLayoutName = undefined;
                    var mySections = TMUtilities.collectionToArray(myDocument.sections);
                    var mySectionCount = mySections.length;
                    for (var j = 0; j < mySectionCount; j++) {
                        var mySection = mySections[j];
                        if (mySection.tmLayoutName()) {
                            myLayoutName = mySection.tmLayoutName();
                        }
                        var myLayoutDimensions = mySection.tmLayoutDimensions(myLayoutName);
                        if (myDeviceType !== undefined) {
                            if (!TMUtilities.itemInArray(this.exportPublicationLayout, myLayoutName)) {
                                TMLogger.info("  ==== Skipping layout: " + myLayoutName + ", not needed to preview on " + myDeviceType + " ====");
                                continue;
                            }
                        }
                        if (!TMUtilities.itemInArray(this.supportedLayouts, myLayoutName)) {
                            TMLogger.info("  ==== Skipping layout: " + myLayoutName + ", unsupported layout name ====");
                            continue;
                        }
                        if (myDocument.tmUsesAlternateLayouts()) {
                            var myPageCount = mySection.alternateLayoutLength;
                            var myOrientation = mySection.tmOrientation();
                            var myPageRange = ExportRangeOrAllPages.EXPORT_RANGE;
                            var myPageString = mySection.alternateLayout;
                        } else {
                            var myPageCount = myDocument.pages.count();
                            var myOrientation = myDocument.tmOrientation();
                            var myPageRange = ExportRangeOrAllPages.EXPORT_ALL;
                            var myPageString = '';
                        }
                        var mySectionPathRel = myLayoutDimensions + '/' + myArticleName + '/' + myOrientation;
                        var mySectionPathFull = myTmpPublication.fsName + '/' + mySectionPathRel;
                        TMLogger.info("  ==== Layout: " + myLayoutName + ' (' + myOrientation + ') ====');
                        if (!myData[myLayoutDimensions]) {
                            myData[myLayoutDimensions] = {};
                        }
                        if (!myData[myLayoutDimensions][myArticleName]) {
                            myData[myLayoutDimensions][myArticleName] = {
                                showInScrubber: myArticleInScrubber,
                                backgroundMusicPlaylist: backgroundMusicPlaylist,
                            };
                        }
                        if (!myData[myLayoutDimensions][myArticleName][myOrientation]) {
                            myData[myLayoutDimensions][myArticleName][myOrientation] = {
                                title: myArticleTitle,
                                tagline: myArticleTagLine,
                                articleURL: myArticleURL,
                                author: myArticleAuthor,
                                pageCount: mySection.tmPages().length,
                                pages: {}
                            };
                        }
                        var myPageLabel = (mySection.tmPages().length == 1) ? 'page' : 'pages';
                        this.exportFullPages(mySection, mySectionPathRel, mySectionPathFull, myPageRange, myPageString, myPageCount);
                        this.exportThumbnails(myDocument, mySection, mySectionPathRel, mySectionPathFull);
                        var mySectionPages = mySection.tmPages();
                        var mySectionPageCount = mySectionPages.length;
                        for (var k = 0; k < mySectionPageCount; k++) {
                            var myPage = mySectionPages[k];
                            myPageNumber++;
                            TMProgressBar.updateLabel(TMUtilities.decodeURI(myFile.name) + ' (page ' + myPageNumber + '/' + myDocument.pages.count() + ')');
                            myPage.tmPrepareForExport(true);
                            myPage.tmParentDocument().tmUnlockAllLayers();
                            myPage.layoutRule = LayoutRuleOptions.OFF;
                            myPage.parent.parent.tmUnlockAllLayers();
                            var myFrame = myPage.textFrames.add();
                            myFrame.geometricBounds = myPage.bounds;
                            var myPageData = {
                                x: 0,
                                y: 0,
                                id: myPage.id,
                                number: myPage.tmRelativePageNumber(),
                                full: mySectionPathRel + '_full' + myPage.tmRelativePageNumber() + '.jpg',
                                width: myPage.tmPageWidthPx(),
                                height: myPage.tmPageHeightPx(),
                                textContents: myPage.tmTextContents(),
                            };
                            if (kEXPORT_FORMAT == 'PDF') {
                                myPageData['full'] = mySectionPathRel + '_full.pdf';
                            }
                            var myPagePathRel = mySectionPathRel + '_' + myPage.tmRelativePageNumber();
                            var myPagePathFull = mySectionPathFull + '_' + myPage.tmRelativePageNumber();
                            myPageData['thumb'] = mySectionPathRel + '_thumb' + myPage.tmRelativePageNumber() + '.jpg';
                            if (myDocument.tmCustomThumbnailPath() != undefined) {
                                myPageData['thumb'] = mySectionPathRel + '_thumb.jpg';
                            }
                            TMProgressBar.updateSubLabel("Processing page " + myPage.tmRelativePageNumber());
                            TMLogger.info("    Processing page " + myPage.tmRelativePageNumber());
                            myPageData['weblinks'] = myPage.tmWebLinks();
                            myPageData['pagelinks'] = myPage.tmPageLinks(undefined, myOptions['horizontalOnly']);
                            myPageData['actions'] = myPage.tmActions(myTmpPublication, myPagePathRel, myPagePathFull, undefined);
                            myPageData['movies'] = myPage.tmMovies(undefined, myTmpPublication);
                            myPageData['sounds'] = myPage.tmSounds(undefined, myTmpPublication);
                            myPageData['webviewers'] = myPage.tmWebViewers(myTmpPublication, myPagePathRel, undefined);
                            myPageData['weboverlays'] = myPage.tmWebOverlays(undefined);
                            myPageData['imagesequences'] = myPage.tmImageSequences(myTmpPublication);
                            myPageData['scrollables'] = myPage.tmScrollables(myTmpPublication, myPagePathRel, myPagePathFull, myOptions);
                            myPageData['multistates'] = myPage.tmSlideShows(myTmpPublication, myPagePathRel, myPagePathFull, myOptions, myOrientation, this.emptyPngName);
                            myData[myLayoutDimensions][myArticleName][myOrientation]['pages'][myPage.id] = myPageData;
                        };
                    };
                    TMProgressBar.updateSubLabel("Closing: " + TMUtilities.decodeURI(myFile.fullName.name));
                    TMLogger.info("Closing: " + myFile.fullName.fsName);
                    myDocument.close(SaveOptions.NO);
                } else {
                    TMLogger.error("Failed to open: " + myFile.fullName.fsName);
                }
                TMProgressBar.updateProgressBySteps(1);
            }
            if (articlesInScrubber == false) {
                myOptions['scrubber'] = false;
            }
            myOptions['alternateLayouts'] = myBook.tmUsesAlternateLayouts();
            this.exportPublicationXml(myTmpPublication, myData, myOptions);
            TMProgressBar.updateProgressBySteps(myPublicationXmlSteps);
        } catch (myException) {
            TMLogger.exception('TMExporter.exportPublication', myException + ' (tm_exporter:350)');
            TMProgressBar.close();
            TMTimer.printElapsed('export', 'Export duration');
            TMStackTrace.addToStack('TMExporter.exportPublication', myException);
        }
        TMProgressBar.updateLabel(TMUtilities.decodeURI(app.activeBook.name));
        TMProgressBar.updateSubLabel("Optimizing publication");
        TMLogger.info('Optimizing publication: ' + TMUtilities.decodeURI(app.activeBook.name));
        TMHelper.call('publication/postProcess', {
            path: myTmpPublication.fsName,
            deviceType: this.exportDeviceType,
            exportFormat: kEXPORT_FORMAT
        })
        TMProgressBar.updateProgressBySteps(1);
        TMLogger.info('Renaming: ' + myTmpPublication.fsName);
        TMLogger.info('      To: ' + myDstPublication.fsName);
        TMFiles.move(myTmpPublication.fsName, myDstPublication.fsName);
        TMProgressBar.close();
        TMApplication.deselectAllObjects();
        TMLogger.info('Publication export was successful');
        TMTimer.printElapsed('export', 'Export duration');
        this.finishExport('exportPublication');
        return true;
    },
    exportArticle: function (myArticlePath, myRootFolderName, myOptions, closeProgress) {
        try {
            TMTimer.start('exportArticle', "Starting new article export to: " + myRootFolderName);
            TMLogger.info('Export format: ' + kEXPORT_FORMAT);
            this.prepareForExport('exportArticle');
            TMBook.resetCache();
            TMCache.reset();
            var myDeviceType = myOptions['previewDeviceType'];
            if (myDeviceType) {
                TMLogger.info('Exporting for device type: ' + myDeviceType);
                this.exportDeviceType = myDeviceType;
            }
            var myData = {};
            var myExportFolder = new Folder(Folder.temp.fsName + '/' + kTMP_PREFIX + TMUtilities.uuid());
            myExportFolder.create();
            TMLogger.debug("preparing media resources")
            this.prepareMediaResources(myExportFolder);
            if (closeProgress != false) {
                TMProgressBar.create("Export Article", 'Exporting article...', 4);
            } else {
                TMProgressBar.updateProgress(0);
            }
            var myArticleFile = new File(myArticlePath);
            TMLogger.debug("preparing web resources")
            var webResourcesFolder = new Folder(myArticleFile.parent.fsName + "/WebResources");
            if (webResourcesFolder.exists || app.books.length == 0) {
                this.emptyPngName = this.exportWebResources(myArticleFile.parent.fsName, myExportFolder);
            } else {
                this.emptyPngName = this.exportWebResources(app.activeBook.filePath.fsName, myExportFolder);
            }
            //
            //
            TMLogger.debug("finished web resources");
            TMProgressBar.updateLabel(TMUtilities.decodeURI(myArticleFile.name));
            TMProgressBar.updateProgressBySteps(1);
            var mySrcArticleFile = new File(myArticlePath);
            TMExporter.documentChecksum = TMHelper.call('fs/checksum', {
                path: myArticlePath
            }).checksum;
            TMExporter.articleName = TMUtilities.decodeURI(mySrcArticleFile.name);
            TMExporter.articlePath = mySrcArticleFile.fsName;
            TMExporter.articleParentPath = mySrcArticleFile.parent.fsName;
            TMLogger.info("Opening: " + TMUtilities.decodeURI(TMExporter.articleName));
            var myDocument = undefined;
            if (TMApplication.isServer()) {
                myDocument = app.open(new File(mySrcArticleFile), OpenOptions.OPEN_COPY)
            } else {
                myDocument = app.open(new File(mySrcArticleFile), kKEEP_TEMP_PAGES, OpenOptions.OPEN_COPY)
            }
            if (myDocument) {
                var documentProperties = myDocument.tmProperties();
                documentProperties['originalFileName'] = myArticleFile.fullName;
                documentProperties['originalBaseName'] = TMUtilities.decodeURI(myArticleFile.name);
                myDocument.label = documentProperties.toSource();
                if (myDeviceType) {
                    this.exportPublicationLayout = [];
                    this.exportPublicationLayouts = [];
                    var mySections = TMUtilities.collectionToArray(myDocument.sections);
                    var mySectionCount = mySections.length;
                    for (var j = 0; j < mySectionCount; j++) {
                        var mySection = mySections[j];
                        var myLayoutName = mySection.tmLayoutName();
                        this.exportPublicationLayouts.push(myLayoutName);
                    }
                    this.determineExportLayouts(myDeviceType);
                    TMLogger.info("Preview Device Type: " + myDeviceType);
                    TMLogger.info("Exporting layouts: " + this.exportPublicationLayout.toSource());
                    if (this.exportPublicationLayout.length == 0) {
                        myDocument.close(SaveOptions.NO);
                        TMProgressBar.close();
                        TMLogger.error("Found no compatible layouts to preview.");
                        TMDialogs.error("Found no compatible layouts to preview.");
                        this.finishExport('exportArticle');
                        return;
                    }
                }
                var pageCount = myDocument.pages.length;
                var myPageNumber = 0;
                TMLogger.debug("  Checksum: " + myDocument.tmChecksum());
                TMLogger.debug("  Uses alternate layouts: " + myDocument.tmUsesAlternateLayouts());
                TMPageItem.tmClearCache();
                myDocument.tmPrepareForExport(true);
                var backgroundMusicPlaylist = myDocument.tmCopyBackgroundMusic(myExportFolder);
                gBaseResolution = myDocument.tmDocumentBaseResolution();
                var myArticleName = myDocument.tmArticleName();
                var myArticleTitle = myDocument.tmArticleTitle();
                var myArticleTagLine = myDocument.tmArticleTagLine();
                var myArticleAuthor = myDocument.tmArticleAuthor();
                var myArticleURL = myDocument.tmArticleURL();
                var myArticleInScrubber = myDocument.tmShowInScrubber();
                var myArticlePageCount = myDocument.pages.count();
                myOptions['name'] = myArticleName;
                if (myOptions['name'] == '') {
                    myOptions['name'] = myArticleTitle;
                }
                var myLayoutName = undefined;
                var mySections = TMUtilities.collectionToArray(myDocument.sections);
                var mySectionCount = mySections.length;
                for (var j = 0; j < mySectionCount; j++) {
                    var mySection = mySections[j];
                    if (mySection.tmLayoutName()) {
                        myLayoutName = mySection.tmLayoutName();
                    }
                    var myLayoutDimensions = mySection.tmLayoutDimensions(myLayoutName);
                    if (myDeviceType !== undefined) {
                        if (!TMUtilities.itemInArray(this.exportPublicationLayout, myLayoutName)) {
                            TMLogger.info("  ==== Skipping layout: " + myLayoutName + ", not needed to preview on " + myDeviceType + " ====");
                            continue;
                        }
                    }
                    if (!TMUtilities.itemInArray(this.supportedLayouts, myLayoutName)) {
                        TMLogger.info("  ==== Skipping layout: " + myLayoutName + ", unsupported layout name ====");
                        continue;
                    }
                    if (myDocument.tmUsesAlternateLayouts() == true) {
                        var myPageCount = mySection.alternateLayoutLength;
                        var myOrientation = mySection.tmOrientation();
                        var myPageRange = ExportRangeOrAllPages.EXPORT_RANGE;
                        var myPageString = mySection.alternateLayout;
                    } else {
                        var myPageCount = myDocument.pages.count();
                        var myOrientation = myDocument.tmOrientation();
                        var myPageRange = ExportRangeOrAllPages.EXPORT_ALL;
                        var myPageString = '';
                    }
                    var mySectionPathRel = myLayoutDimensions + '/' + myArticleName + '/' + myOrientation;
                    var mySectionPathFull = myExportFolder.fsName + '/' + mySectionPathRel;
                    TMLogger.info("  ==== Layout: " + myLayoutName + ' (' + myOrientation + ') ====');
                    if (!myData[myLayoutDimensions]) {
                        myData[myLayoutDimensions] = {};
                    }
                    if (!myData[myLayoutDimensions][myArticleName]) {
                        myData[myLayoutDimensions][myArticleName] = {
                            showInScrubber: myArticleInScrubber,
                            backgroundMusicPlaylist: backgroundMusicPlaylist,
                        };
                    }
                    if (!myData[myLayoutDimensions][myArticleName][myOrientation]) {
                        myData[myLayoutDimensions][myArticleName][myOrientation] = {
                            title: myArticleTitle,
                            tagline: myArticleTagLine,
                            articleURL: myArticleURL,
                            author: myArticleAuthor,
                            pageCount: mySection.tmPages().length,
                            pages: {}
                        };
                    }
                    this.exportFullPages(mySection, mySectionPathRel, mySectionPathFull, myPageRange, myPageString, myPageCount);
                    this.exportThumbnails(myDocument, mySection, mySectionPathRel, mySectionPathFull);
                    var mySectionPages = mySection.tmPages();
                    var mySectionPageCount = mySectionPages.length;
                    for (var k = 0; k < mySectionPageCount; k++) {
                        var myPage = mySectionPages[k];
                        myPageNumber++;
                        TMProgressBar.updateLabel(TMUtilities.decodeURI(myArticleFile.name) + ' (page ' + myPageNumber + '/' + myArticlePageCount + ')');
                        myPage.tmPrepareForExport(true);
                        myPage.tmParentDocument().tmUnlockAllLayers();
                        myPage.layoutRule = LayoutRuleOptions.OFF;
                        myPage.parent.parent.tmUnlockAllLayers();
                        var myPageData = {
                            x: 0,
                            y: 0,
                            id: myPage.id,
                            number: myPage.tmRelativePageNumber(),
                            full: mySectionPathRel + '_full' + myPage.tmRelativePageNumber() + '.jpg',
                            width: myPage.tmPageWidthPx(),
                            height: myPage.tmPageHeightPx(),
                            textContents: myPage.tmTextContents(),
                        };
                        if (kEXPORT_FORMAT == 'PDF') {
                            myPageData['full'] = mySectionPathRel + '_full.pdf';
                        }
                        var myPagePathRel = mySectionPathRel + '_' + myPage.tmRelativePageNumber();
                        var myPagePathFull = mySectionPathFull + '_' + myPage.tmRelativePageNumber();
                        myPageData['thumb'] = mySectionPathRel + '_thumb' + myPage.tmRelativePageNumber() + '.jpg';
                        if (myDocument.tmCustomThumbnailPath()) {
                            myPageData['thumb'] = mySectionPathRel + '_thumb.jpg';
                        }
                        TMProgressBar.updateSubLabel("Processing page " + myPage.tmRelativePageNumber());
                        TMLogger.info("    Processing page " + myPage.tmRelativePageNumber());
                        myPageData['weblinks'] = myPage.tmWebLinks();
                        myPageData['pagelinks'] = myPage.tmPageLinks(undefined, myOptions['horizontalOnly']);
                        myPageData['actions'] = myPage.tmActions(myExportFolder.fsName, myPagePathRel, myPagePathFull, undefined);
                        myPageData['movies'] = myPage.tmMovies(undefined, myExportFolder.fsName);
                        myPageData['sounds'] = myPage.tmSounds(undefined, myExportFolder.fsName);
                        myPageData['webviewers'] = myPage.tmWebViewers(myExportFolder.fsName, myPagePathRel, undefined);
                        myPageData['weboverlays'] = myPage.tmWebOverlays(undefined);
                        myPageData['imagesequences'] = myPage.tmImageSequences(myExportFolder.fsName);
                        myPageData['scrollables'] = myPage.tmScrollables(myExportFolder.fsName, myPagePathRel, myPagePathFull, myOptions);
                        myPageData['multistates'] = myPage.tmSlideShows(myExportFolder.fsName, myPagePathRel, myPagePathFull, myOptions, myOrientation, this.emptyPngName);
                        myData[myLayoutDimensions][myArticleName][myOrientation]['pages'][myPage.id] = myPageData;
                    };
                };
                TMProgressBar.updateProgressBySteps(1);
                TMLogger.info("Finished: " + myArticlePath);
                if (kKEEP_TEMP_PAGES == false) {
                    myDocument.close(SaveOptions.NO);
                }
            } else {
                TMLogger.error("Failed to open: " + mySrcArticleFile.fsName);
                if (closeProgress != false) {
                    TMProgressBar.close();
                }
                this.finishExport('exportArticle');
                return undefined;
            }
            this.exportPublicationXml(myExportFolder.fsName, myData, myOptions);
            TMProgressBar.updateProgressBySteps(1);
            TMProgressBar.updateLabel(TMUtilities.decodeURI(myArticleFile.name));
            TMProgressBar.updateSubLabel("Optimizing article");
            TMLogger.info('Optimizing article: ' + TMUtilities.decodeURI(myArticleFile.name));
            TMHelper.call('publication/postProcess', {
                path: myExportFolder.fsName,
                deviceType: this.exportDeviceType,
                exportFormat: kEXPORT_FORMAT
            })
            TMProgressBar.updateProgressBySteps(1);
            TMLogger.info('Renaming: ' + myExportFolder.fsName);
            TMLogger.info('      To: ' + myRootFolderName);
            TMFiles.move(myExportFolder.fsName, myRootFolderName);
            if (closeProgress != false) {
                TMProgressBar.close();
            }
        } catch (myException) {
            if (closeProgress != false) {
                TMProgressBar.close();
            }
            TMTimer.printElapsed('exportArticle', 'Export duration');
            TMStackTrace.addToStack('TMExporter.exportArticle', myException);
        }
        TMApplication.deselectAllObjects();
        TMLogger.info('Article export was successful');
        TMTimer.printElapsed('exportArticle', 'Export duration');
        this.finishExport('exportArticle');
        return true;
    },
    determineExportLayouts: function (myDeviceType) {
        if (myDeviceType == "ipad" || myDeviceType == "ipad-retina") {
            if (TMUtilities.itemInArray(this.exportPublicationLayouts, 'iPad H')) {
                this.exportPublicationLayout.push('ipad h');
            }
            if (TMUtilities.itemInArray(this.exportPublicationLayouts, 'iPad V')) {
                this.exportPublicationLayout.push('ipad v');
            }
        }
        if (myDeviceType == "android10") {
            if (TMUtilities.itemInArray(this.exportPublicationLayouts, 'Android 10" H')) {
                this.exportPublicationLayout.push('android 10" h');
            }
            if (TMUtilities.itemInArray(this.exportPublicationLayouts, 'Android 10" V')) {
                this.exportPublicationLayout.push('android 10" v');
            }
            if (this.exportPublicationLayout.length == 0) {
                if (TMUtilities.itemInArray(this.exportPublicationLayouts, 'iPad H')) {
                    this.exportPublicationLayout.push('ipad h');
                }
                if (TMUtilities.itemInArray(this.exportPublicationLayouts, 'iPad V')) {
                    this.exportPublicationLayout.push('ipad v');
                }
            }
            if (this.exportPublicationLayout.length == 0) {
                if (TMUtilities.itemInArray(this.exportPublicationLayouts, 'Kindle Fire/Nook H')) {
                    this.exportPublicationLayout.push('kindle fire/nook h');
                }
                if (TMUtilities.itemInArray(this.exportPublicationLayouts, 'Kindle Fire/Nook V')) {
                    this.exportPublicationLayout.push('kindle fire/nook v');
                }
            }
        }
        if (myDeviceType == "android7") {
            if (TMUtilities.itemInArray(this.exportPublicationLayouts, 'Kindle Fire/Nook H')) {
                this.exportPublicationLayout.push('kindle fire/nook h');
            }
            if (TMUtilities.itemInArray(this.exportPublicationLayouts, 'Kindle Fire/Nook V')) {
                this.exportPublicationLayout.push('kindle fire/nook v');
            }
            if (this.exportPublicationLayout.length == 0) {
                if (TMUtilities.itemInArray(this.exportPublicationLayouts, 'Android 10" H')) {
                    this.exportPublicationLayout.push('android 10" h');
                }
                if (TMUtilities.itemInArray(this.exportPublicationLayouts, 'Android 10" V')) {
                    this.exportPublicationLayout.push('android 10" v');
                }
            }
            if (this.exportPublicationLayout.length == 0) {
                if (TMUtilities.itemInArray(this.exportPublicationLayouts, 'iPad H')) {
                    this.exportPublicationLayout.push('ipad h');
                }
                if (TMUtilities.itemInArray(this.exportPublicationLayouts, 'iPad V')) {
                    this.exportPublicationLayout.push('ipad v');
                }
            }
        }
        if (myDeviceType == "phone_s" || myDeviceType == "phone_m" || myDeviceType == "phone_l") {
            if (TMUtilities.itemInArray(this.exportPublicationLayouts, 'Phone H')) {
                this.exportPublicationLayout.push('phone h');
            }
            if (TMUtilities.itemInArray(this.exportPublicationLayouts, 'Phone V')) {
                this.exportPublicationLayout.push('phone v');
            }
        }
    },
    prepareMediaResources: function (myTmpPublication) {
        try {
            var myMedia = new Folder(myTmpPublication.fsName + "/MediaResources");
            if (!myMedia.exists) {
                myMedia.create();
            }
        } catch (myException) {
            TMLogger.exception("TMExporter.prepareMediaResources", myException + ' (tm_exporter:765)');
        }
    },
    exportWebResources: function (myFilePath, myTempFolder) {
        try {
            var webResourcesFolder = new Folder(myFilePath + "/WebResources");
            TMLogger.debug("WebResources: " + webResourcesFolder.fsName);
            if (!webResourcesFolder.exists) {
                webResourcesFolder.create();
            }
            var webResourcesSize = TMFiles.getFolderSize(myFilePath + "/WebResources");
            webResourcesSize = TMUtilities.formatFileSize(webResourcesSize, 0);
            TMLogger.info("Copying the WebResources (" + webResourcesSize + ")");
            TMProgressBar.updateLabel("Copying WebResources (" + webResourcesSize + ")");
            var result = TMHelper.call('fs/copyWebResources', {
                fromPath: myFilePath + '/WebResources',
                toPath: myTempFolder.fsName + '/WebResources'
            });
            TMLogger.debug('Empty PNG Name: ' + result.emptyPngName);
            return result.emptyPngName;
        } catch (myException) {
            TMLogger.exception("TMExporter.exportWebResources", myException + ' (tm_exporter:790)');
        }
    },
    exportFullPages: function (mySection, myPathRel, myPathFull, myPageRange, myPageString, myPageCount) {
        try {
            var myExtension = (kEXPORT_FORMAT == 'PDF') ? '.pdf' : '.jpg';
            var myDocument = mySection.parent;
            var myExportFile = new File(myPathFull + '_full' + myExtension);
            var myExportFileRetina = new File(myPathFull + '_full@2x' + myExtension);
            var myPageLabel = (mySection.tmPages().length == 1) ? 'page' : 'pages';
            var myExportFolder = new File(myPathFull);
            if (mySection.tmLayoutDimensions() == '568x320') {
                var folder = new Folder(myExportFolder.parent.fsName.replace('568x320', '736x414'));
                if (!folder.exists) {
                    folder.create();
                }
            } else {
                if (!myExportFolder.parent.exists) {
                    myExportFolder.parent.create();
                }
            }
            myDocument.tmHideInteractiveContents();
            TMLogger.info("    Exporting full pages for " + mySection.tmLayoutName())
            TMProgressBar.updateSubLabel("Exporting full pages for " + mySection.tmLayoutName() + " (" + mySection.tmPages().length + " " + myPageLabel + ")");
            TMSetup.exportFullJPG();
            app.jpegExportPreferences.jpegExportRange = myPageRange;
            app.jpegExportPreferences.pageString = myPageString;
            app.pdfExportPreferences.pageRange = mySection.tmLayoutName();
            if (mySection.tmLayoutDimensions() == '1024x768' && (!this.exportDeviceType || !this.exportDeviceType.tmStartsWith('phone_'))) {
                if (kEXPORT_FORMAT == 'JPG') {
                    if (!TMCache.copyCachedImages(myDocument, mySection.tmLayoutDimensions(), myExportFileRetina.parent.fsName, 'full_' + mySection.tmOrientation(), '')) {
                        app.jpegExportPreferences.exportResolution = Math.ceil(gBaseResolution * 2);
                        myDocument.exportFile(ExportFormat.JPG, myExportFileRetina);
                        if (!myExportFileRetina.exists) {
                            var myException = new Error('Failed to export: ' + myExportFileRetina);
                            TMStackTrace.addToStack("TMExporter.exportFullPages", myException);
                        }
                        TMFiles.renameRetinaImages(myPathFull + '_full', myPageCount, 'jpg');
                        TMCache.cacheImages(myDocument, mySection.tmLayoutDimensions(), myExportFileRetina.parent.fsName, 'full_' + mySection.tmOrientation(), mySection.tmOrientation() + "_full", '.jpg');
                    }
                }
                if (kEXPORT_FORMAT == 'PDF') {
                    if (!TMCache.copyCachedImages(myDocument, mySection.tmLayoutDimensions(), myExportFile.parent.fsName, 'full_' + mySection.tmOrientation(), '')) {
                        TMSetup.exportFullPDF(Math.ceil(gBaseResolution * 2));
                        myDocument.exportFile(ExportFormat.PDF_TYPE, myExportFile);
                        TMCache.cacheImages(myDocument, mySection.tmLayoutDimensions(), myExportFile.parent.fsName, 'full_' + mySection.tmOrientation(), mySection.tmOrientation() + "_full", '.pdf');
                    }
                }
            }
            if (mySection.tmLayoutDimensions() == '1280x800' && (!this.exportDeviceType || this.exportDeviceType.tmStartsWith('android'))) {
                if (kEXPORT_FORMAT == 'JPG') {
                    if (!TMCache.copyCachedImages(myDocument, mySection.tmLayoutDimensions(), myExportFile.parent.fsName, 'full_' + mySection.tmOrientation(), '')) {
                        app.jpegExportPreferences.exportResolution = Math.ceil(gBaseResolution);
                        myDocument.exportFile(ExportFormat.JPG, myExportFile);
                        if (!myExportFile.exists) {
                            var myException = new Error('Failed to export: ' + myExportFile);
                            TMStackTrace.addToStack("TMExporter.exportFullPages", myException);
                        }
                        TMFiles.renameRetinaImages(myPathFull + '_full', myPageCount, 'jpg');
                        TMCache.cacheImages(myDocument, mySection.tmLayoutDimensions(), myExportFile.parent.fsName, 'full_' + mySection.tmOrientation(), mySection.tmOrientation() + "_full", '.jpg');
                    }
                }
                if (kEXPORT_FORMAT == 'PDF') {
                    if (!TMCache.copyCachedImages(myDocument, mySection.tmLayoutDimensions(), myExportFile.parent.fsName, 'full_' + mySection.tmOrientation(), '')) {
                        TMSetup.exportFullPDF(Math.ceil(gBaseResolution * 2));
                        myDocument.exportFile(ExportFormat.PDF_TYPE, myExportFile);
                        TMCache.cacheImages(myDocument, mySection.tmLayoutDimensions(), myExportFile.parent.fsName, 'full_' + mySection.tmOrientation(), mySection.tmOrientation() + "_full", '.pdf');
                    }
                }
            }
            if (mySection.tmLayoutDimensions() == '1024x600' && (!this.exportDeviceType || this.exportDeviceType.tmStartsWith('android'))) {
                if (kEXPORT_FORMAT == 'JPG') {
                    if (!TMCache.copyCachedImages(myDocument, mySection.tmLayoutDimensions(), myExportFile.parent.fsName, 'full_' + mySection.tmOrientation(), '')) {
                        app.jpegExportPreferences.exportResolution = gBaseResolution;
                        myDocument.exportFile(ExportFormat.JPG, myExportFile);
                        if (!myExportFile.exists) {
                            var myException = new Error('Failed to export: ' + myExportFile);
                            TMStackTrace.addToStack("TMExporter.exportFullPages", myException);
                        }
                        TMFiles.renameRetinaImages(myPathFull + '_full', myPageCount, 'jpg');
                        TMCache.cacheImages(myDocument, mySection.tmLayoutDimensions(), myExportFile.parent.fsName, 'full_' + mySection.tmOrientation(), mySection.tmOrientation() + "_full", '.jpg');
                    }
                }
                if (kEXPORT_FORMAT == 'PDF') {
                    if (!TMCache.copyCachedImages(myDocument, mySection.tmLayoutDimensions(), myExportFile.parent.fsName, 'full_' + mySection.tmOrientation(), '')) {
                        TMSetup.exportFullPDF(Math.ceil(gBaseResolution));
                        myDocument.exportFile(ExportFormat.PDF_TYPE, myExportFile);
                        TMCache.cacheImages(myDocument, mySection.tmLayoutDimensions(), myExportFile.parent.fsName, 'full_' + mySection.tmOrientation(), mySection.tmOrientation() + "_full", '.pdf');
                    }
                }
            }
            if (mySection.tmLayoutDimensions() == '568x320' && (!this.exportDeviceType || this.exportDeviceType.tmStartsWith('phone_'))) {
                myExportFile = new File(myExportFile.fsName.replace('568x320', '736x414').replace('@2x', '@3x'));
                myExportFileRetina = new File(myExportFileRetina.fsName.replace('568x320', '736x414').replace('@2x', '@3x'));
                if (kEXPORT_FORMAT == 'JPG') {
                    if (!TMCache.copyCachedImages(myDocument, '736x414', myExportFileRetina.parent.fsName.replace('568x320', '736x414'), 'full_' + mySection.tmOrientation(), '@3x')) {
                        app.jpegExportPreferences.exportResolution = Math.ceil(gBaseResolution * (2208 / 568));
                        myDocument.exportFile(ExportFormat.JPG, myExportFileRetina);
                        if (!myExportFileRetina.exists) {
                            var myException = new Error('Failed to export: ' + myExportFileRetina);
                            TMStackTrace.addToStack("TMExporter.exportFullPages", myException);
                        }
                        TMFiles.renameRetinaImages(myPathFull.replace('568x320', '736x414') + '_full', myPageCount, 'jpg');
                        TMCache.cacheImages(myDocument, '736x414', myExportFileRetina.parent.fsName.replace('568x320', '736x414'), 'full_' + mySection.tmOrientation(), mySection.tmOrientation() + "_full", '@3x.jpg');
                    }
                }
                if (kEXPORT_FORMAT == 'PDF') {
                    if (!TMCache.copyCachedImages(myDocument, mySection.tmLayoutDimensions(), myExportFile.parent.fsName, 'full_' + mySection.tmOrientation(), '')) {
                        TMSetup.exportFullPDF(Math.ceil(gBaseResolution * (2208 / 568)));
                        myDocument.exportFile(ExportFormat.PDF_TYPE, myExportFile);
                        TMCache.cacheImages(myDocument, mySection.tmLayoutDimensions(), myExportFile.parent.fsName, 'full_' + mySection.tmOrientation(), mySection.tmOrientation() + "_full", '.pdf');
                    }
                }
            }
            myDocument.tmRestoreInteractiveContents();
        } catch (myException) {
            TMLogger.exception("TMExporter.exportFullPages", myException + ' (tm_exporter:970)');
        }
    },
    exportThumbnails: function (myDocument, mySection, myPathRel, myPathFull) {
        try {
            var myPageLabel = (mySection.tmPages().length == 1) ? 'page' : 'pages';
            var myPageCount = mySection.tmPages().length;
            var myDocument = mySection.parent;
            var myExportFile = new File(myPathFull + '_thumb.jpg');
            var myExportFileRetina = new File(myPathFull + '_thumb@2x.jpg');
            if (kEXPORT_FORMAT == 'PDF') {
                myExportFileRetina = myExportFile;
            }
            myDocument.tmHideInteractiveContents();
            TMLogger.info("    Exporting thumbnails for " + mySection.tmLayoutName())
            TMProgressBar.updateSubLabel("Exporting thumbnails for " + mySection.tmLayoutName() + " (" + myPageCount + " " + myPageLabel + ")");
            TMSetup.exportFullJPG();
            app.jpegExportPreferences.jpegExportRange = ExportRangeOrAllPages.EXPORT_RANGE;
            app.jpegExportPreferences.pageString = mySection.alternateLayout;
            if (mySection.tmLayoutDimensions() == '1024x768' && (!this.exportDeviceType || !this.exportDeviceType.tmStartsWith('phone_'))) {
                if (!TMCache.copyCachedImages(myDocument, mySection.tmLayoutDimensions(), myExportFileRetina.parent.fsName, 'thumb_' + mySection.tmOrientation(), '')) {
                    app.jpegExportPreferences.exportResolution = Math.ceil((gBaseResolution / 4) * 2);
                    myDocument.exportFile(ExportFormat.JPG, myExportFileRetina);
                    if (!myExportFileRetina.exists) {
                        var myException = new Error('Failed to export: ' + myExportFileRetina);
                        TMStackTrace.addToStack("TMExporter.exportThumbnails", myException);
                    }
                    TMFiles.renameRetinaImages(myPathFull + '_thumb', myPageCount, 'jpg');
                    TMCache.cacheImages(myDocument, mySection.tmLayoutDimensions(), myExportFileRetina.parent.fsName, 'thumb_' + mySection.tmOrientation(), mySection.tmOrientation() + "_thumb", '.jpg');
                }
            }
            if (mySection.tmLayoutDimensions() == '1280x800' && (!this.exportDeviceType || this.exportDeviceType.tmStartsWith('android'))) {
                if (!TMCache.copyCachedImages(myDocument, mySection.tmLayoutDimensions(), myExportFile.parent.fsName, 'thumb_' + mySection.tmOrientation(), '')) {
                    app.jpegExportPreferences.exportResolution = Math.ceil((gBaseResolution / 4) * 2);
                    myDocument.exportFile(ExportFormat.JPG, myExportFile);
                    if (!myExportFile.exists) {
                        var myException = new Error('Failed to export: ' + myExportFile);
                        TMStackTrace.addToStack("TMExporter.exportThumbnails", myException);
                    }
                    TMFiles.renameRetinaImages(myPathFull + '_thumb', myPageCount, 'jpg');
                    TMCache.cacheImages(myDocument, mySection.tmLayoutDimensions(), myExportFile.parent.fsName, 'thumb_' + mySection.tmOrientation(), mySection.tmOrientation() + "_thumb", '.jpg');
                }
            }
            if (mySection.tmLayoutDimensions() == '1024x600' && (!this.exportDeviceType || this.exportDeviceType.tmStartsWith('android'))) {
                if (!TMCache.copyCachedImages(myDocument, mySection.tmLayoutDimensions(), myExportFile.parent.fsName, 'thumb_' + mySection.tmOrientation(), '')) {
                    app.jpegExportPreferences.exportResolution = Math.ceil(gBaseResolution / 4);
                    myDocument.exportFile(ExportFormat.JPG, myExportFile);
                    if (!myExportFile.exists) {
                        var myException = new Error('Failed to export: ' + myExportFile);
                        TMStackTrace.addToStack("TMExporter.exportThumbnails", myException);
                    }
                    TMFiles.renameRetinaImages(myPathFull + '_thumb', myPageCount, 'jpg');
                    TMCache.cacheImages(myDocument, mySection.tmLayoutDimensions(), myExportFile.parent.fsName, 'thumb_' + mySection.tmOrientation(), mySection.tmOrientation() + "_thumb", '.jpg');
                }
            }
            if (mySection.tmLayoutDimensions() == '568x320' && (!this.exportDeviceType || this.exportDeviceType.tmStartsWith('phone_'))) {
                if (!TMCache.copyCachedImages(myDocument, '736x414', myExportFile.parent.fsName.replace('568x320', '736x414'), 'thumb_' + mySection.tmOrientation(), '@3x')) {
                    myExportFileRetina = new File(myExportFileRetina.fsName.replace('568x320', '736x414').replace('@2x', '@3x'));
                    app.jpegExportPreferences.exportResolution = Math.ceil((gBaseResolution / 4) * (2208 / 568));
                    myDocument.exportFile(ExportFormat.JPG, myExportFileRetina);
                    if (!myExportFileRetina.exists) {
                        var myException = new Error('Failed to export: ' + myExportFileRetina);
                        TMStackTrace.addToStack("TMExporter.exportThumbnails", myException);
                    }
                    TMFiles.renameRetinaImages(myPathFull.replace('568x320', '736x414') + '_thumb', myPageCount, 'jpg');
                    TMCache.cacheImages(myDocument, '736x414', myExportFileRetina.parent.fsName.replace('568x320', '736x414'), 'thumb_' + mySection.tmOrientation(), mySection.tmOrientation() + "_thumb", '@3x.jpg');
                }
            }
            var myThumbSourceFile = myDocument.tmCustomThumbnailPath();
            if (myThumbSourceFile != undefined) {
                TMLogger.info("    Found custom thumbnail: " + myThumbSourceFile.fsName);
                if (mySection.tmLayoutDimensions() == '1024x768' && (!this.exportDeviceType || !this.exportDeviceType.tmStartsWith('phone_'))) {
                    var suffix = (kEXPORT_FORMAT == 'PDF') ? '' : '@2x';
                    TMLogger.info("    Copying custom thumbnail: " + myPathRel + '_thumb' + suffix + '.jpg');
                    TMLogger.debug(myPathFull + '_thumb' + suffix + '.jpg');
                    myThumbSourceFile.copy(new File(myPathFull + '_thumb' + suffix + '.jpg'));
                }
                if (mySection.tmLayoutDimensions() == '1280x800' && (!this.exportDeviceType || this.exportDeviceType.tmStartsWith('android'))) {
                    TMLogger.info("    Copying custom thumbnail: " + myPathRel + '_thumb.jpg');
                    TMLogger.debug(myPathFull + '_thumb.jpg');
                    myThumbSourceFile.copy(new File(myPathFull + '_thumb.jpg'));
                }
                if (mySection.tmLayoutDimensions() == '1024x600' && (!this.exportDeviceType || this.exportDeviceType.tmStartsWith('android'))) {
                    TMLogger.info("    Copying custom thumbnail: " + myPathRel + '_thumb.jpg');
                    TMLogger.debug(myPathFull + '_thumb.jpg');
                    myThumbSourceFile.copy(new File(myPathFull + '_thumb.jpg'));
                }
                if (mySection.tmLayoutDimensions() == '568x320' && (!this.exportDeviceType || this.exportDeviceType.tmStartsWith('phone_'))) {
                    var suffix = (kEXPORT_FORMAT == 'PDF') ? '' : '@3x';
                    TMLogger.info("    Copying custom thumbnail: " + myPathRel.replace('568x320', '736x414') + '_thumb' + suffix + '.jpg');
                    TMLogger.info(myPathFull.replace('568x320', '736x414') + '_thumb' + suffix + '.jpg');
                    myThumbSourceFile.copy(new File(myPathFull.replace('568x320', '736x414') + '_thumb' + suffix + '.jpg'));
                }
            }
            myDocument.tmRestoreInteractiveContents();
        } catch (myException) {
            TMLogger.exception("TMExporter.exportThumbnails", myException + ' (tm_exporter:1107)');
        }
    },
    exportPublicationXml: function (myTmpPublication, myPublicationData, myOptions) {
        try {
            for (var myLayoutName in myPublicationData) {
                var myLayoutData = myPublicationData[myLayoutName];
                if (!myLayoutData) {
                    continue;
                }
                var myXmlName = 'publication';
                if (myLayoutName != "1024x768") {
                    myXmlName += '_' + myLayoutName;
                }
                myXmlName += '.xml';
                var myXmlPath = myTmpPublication + '/' + myXmlName;
                myPublicationProperties = TMUtilities.cloneObject(myOptions);
                myPublicationProperties['layout'] = myLayoutName;
                TMLogger.info('Creating: ' + myXmlPath);
                TMPublicationWriter.writePublication(myPublicationProperties, myLayoutData, myXmlPath);
                if (myLayoutName == '568x320') {
                    TMPublicationWriter.rewritePublication(myXmlPath, '568x320', '568x320');
                    TMPublicationWriter.rewritePublication(myXmlPath, '568x320', '667x375');
                    TMPublicationWriter.rewritePublication(myXmlPath, '568x320', '736x414');
                }
            }
        } catch (myException) {
            TMLogger.silentException("TMExporter.exportPublicationXml", myException);
            throw myException;
        }
    },
    exportTransparentObject: function (myPage, myPageItem, myExportFileName, myOffsetX, myOffsetY, myPaddingX, myPaddingY, myResolution, hScale, vScale, myClipToPage, exportFormat) {
        var myTempPage = undefined;
        try {
            myPageItem.visible = true;
            var myDocument = myPage.parent.parent;
            myOffsetX = TMGeometry.convertToPoints(myOffsetX);
            myOffsetY = TMGeometry.convertToPoints(myOffsetY);
            myPaddingX = TMGeometry.convertToPoints(myPaddingX);
            myPaddingY = TMGeometry.convertToPoints(myPaddingY);
            var myPageBounds = myPage.tmPageBounds();
            var myItemBounds = TMGeometry.getBounds(myPageItem.visibleBounds);
            var myCropX = 0;
            var myCropY = 0;
            if (exportFormat == 'PDF') {
                myExportFileName = myExportFileName.replace('@3x', '').replace('@2x', '');
            }
            if (TMCache.copyCachedAsset(myDocument, myPage.tmLayoutDimensions(), myExportFileName)) {
                return;
            }
            var myExportFile = new File(myExportFileName);
            var myExportFormat = myExportFile.tmFileExtension();
            var myScreenScaling = myExportFileName.tmContains('@2x') ? 2.0 : 1.0;
            this.configureBaselineGridSettings(myPageItem);
            myTempPage = this.createNewPage(myPageItem, myOffsetX, myOffsetY, myPaddingX, myPaddingY, hScale, vScale, myCropX, myCropY);
            if (!myTempPage) {
                return;
            }
            var myNewPageItem = this.copyItemToPage(myTempPage, myPageItem, myOffsetX, myOffsetY, hScale, vScale);
            if (!myNewPageItem) {
                return;
            }
            var myExportPageRange = "+" + (myTempPage.documentOffset + 1);
            myResolution = Math.ceil(myResolution);
            if (exportFormat == 'PDF') {
                app.pdfExportPreferences.pageRange = myExportPageRange;
                TMSetup.exportFullPDF(Math.ceil(myResolution));
                myDocument.exportFile(ExportFormat.PDF_TYPE, myExportFile);
            } else if (myExportFormat == 'png') {
                TMSetup.exportFullPNG();
                app.pngExportPreferences.pngExportRange = ExportRangeOrAllPages.EXPORT_RANGE;
                app.pngExportPreferences.pageString = myExportPageRange;
                app.pngExportPreferences.exportResolution = myResolution;
                myDocument.exportFile(ExportFormat.PNG_FORMAT, myExportFile);
            } else {
                TMSetup.exportFullJPG();
                app.jpegExportPreferences.jpegExportRange = ExportRangeOrAllPages.EXPORT_RANGE;
                app.jpegExportPreferences.pageString = myExportPageRange;
                app.jpegExportPreferences.exportResolution = myResolution;
                myDocument.exportFile(ExportFormat.JPG, myExportFile);
            }
            if (!myExportFile.exists) {
                var myException = new Error('Failed to export: ' + myExportFile);
                TMStackTrace.addToStack("TMExporter.exportTransparentObject", myException);
            }
            TMCache.cacheAsset(myDocument, myPage.tmLayoutDimensions(), myExportFileName);
            this.restoreBaselineGridSettings(myPageItem);
            if (myTempPage.isValid && !kKEEP_TEMP_PAGES) {
                myTempPage.remove();
            }
        } catch (myException) {
            TMLogger.exception("TMExporter.exportTransparentObject(1)", myException + ' (tm_exporter:1229)');
        } finally {
            if (myTempPage && myTempPage.isValid && !kKEEP_TEMP_PAGES) {
                try {
                    myTempPage.remove();
                } catch (myException) {
                    TMLogger.exception("TMExporter.exportTransparentObject(2)", myException + ' (tm_exporter:1235)');
                }
            }
        }
    },
    createNewPage: function (myPageItem, offsetX, offsetY, paddingX, paddingY, hScale, vScale, myCropX, myCropY) {
        try {
            var myDocument = myPageItem.parentPage.parent.parent;
            var myBounds = myPageItem.visibleBounds;
            var myTempSpread = myDocument.spreads.add(LocationOptions.AT_END);
            var myTempPage = TMUtilities.collectionToArray(myDocument.spreads.lastItem().pages)[0];
            this.configurePage(myTempPage);
            if (!myCropX || myCropX < 0) {
                myCropX = 0;
            }
            if (!myCropY || myCropY < 0) {
                myCropY = 0;
            }
            var size = [
                TMGeometry.convertToPoints(myBounds[3] - myBounds[1] + offsetX + paddingX - myCropX) * hScale,
                TMGeometry.convertToPoints(myBounds[2] - myBounds[0] + offsetY + paddingY - myCropY) * vScale
            ];
            try {
                myTempPage.resize(
                    BoundingBoxLimits.GEOMETRIC_PATH_BOUNDS,
                    [0, 0],
                    ResizeMethods.REPLACING_CURRENT_DIMENSIONS_WITH,
                    size,
                    true,
                    false
                );
            } catch (myException) {
                TMLogger.exception("TMExporter.createNewPage", myException + ' (tm_exporter:1297)');
            }
            return myTempPage;
        } catch (myException) {
            TMLogger.exception("TMExporter.createNewPage", myException + ' (tm_exporter:1303)');
            return undefined;
        }
    },
    configurePage: function (myPage) {
        try {
            var myDocument = myPage.parent.parent;
            myDocument.tmUnlockAllLayers();
            try {
                if (myPage.pageItems.length > 0) {
                    myPage.pageItems.everyItem().locked = false;
                }
            } catch (myException) {
                TMLogger.exception("TMExporter.configurePage: Failed to unlock page items: ", myException + ' (tm_exporter:1317)');
            }
            myPage.layoutRule = LayoutRuleOptions.OFF;
            try {
                if (myPage.pageItems.length > 1) {
                    myPage.groups.add(myPage.pageItems);
                }
            } catch (myException) { }
            try {
                myPage.appliedMaster = NothingEnum.NOTHING;
            } catch (myException) { }
            try {
                myPage.marginPreferences.left = 0;
                myPage.marginPreferences.top = 0;
                myPage.marginPreferences.bottom = 0;
                myPage.marginPreferences.right = 0;
                myPage.marginPreferences.columnCount = 1;
            } catch (myException) { }
        } catch (myException) {
            TMLogger.exception("TMExporter.configurePage", myException + ' (tm_exporter:1340)');
        }
    },
    copyItemToPage: function (myPage, myPageItem, offsetX, offsetY, hScale, vScale) {
        try {
            if (myPageItem.locked) {
                myPageItem.locked = false;
            }
            var myNewPageItem = myPageItem.duplicate(myPage);
            if (TMApplication.isServer()) { // Only needed for scrollable content?
                if (TMPageItem.isScrollable(myPageItem.parent)) {
                    hScale = hScale * (parseFloat(myPageItem.horizontalScale) / parseFloat(100));
                    vScale = vScale * (parseFloat(myPageItem.verticalScale) / parseFloat(100));
                }
            }
            myNewPageItem.transform(
                CoordinateSpaces.INNER_COORDINATES,
                AnchorPoint.TOP_LEFT_ANCHOR,
                [hScale, 0, 0, vScale, 0, 0]
            );
            var myNewBounds = myNewPageItem.visibleBounds;
            var x = myNewBounds[1];
            var y = myNewBounds[0];
            myNewPageItem.move([0, 0], [-x, -y]);
            if (offsetX || offsetY) {
                myNewPageItem.move([offsetX, offsetY]);
            }
            return myNewPageItem;
        } catch (myException) {
            TMLogger.exception("TMExporter.copyItemToPage", myException + ' (tm_exporter:1378)');
            return undefined;
        }
    },
    configureBaselineGridSettings: function (myPageItem) {
        try {
            var myPage = myPageItem.parentPage;
            var myDocument = myPage.parent.parent;
            var myPageItemBounds = TMGeometry.getBounds(myPageItem.geometricBounds);
            this.savedBaselineStart = myDocument.gridPreferences.baselineStart;
            this.savedBaselineRelative = myDocument.gridPreferences.baselineGridRelativeOption;
            var baselineOffset = this.savedBaselineStart;
            if (this.savedBaselineRelative == BaselineGridRelativeOption.TOP_OF_MARGIN_OF_BASELINE_GRID_RELATIVE_OPTION) {
                baselineOffset += myPage.marginPreferences.top;
            }
            var baselineOffset = TMGeometry.convertToPixels(baselineOffset);
            var baselineInterval = TMGeometry.convertToPixels(myDocument.gridPreferences.baselineDivision);
            var itemOffset = TMGeometry.convertToPixels(myPageItemBounds['y']);
            var newBaselineOffset = baselineOffset;
            do {
                newBaselineOffset += baselineInterval;
            } while (newBaselineOffset < itemOffset);
            try {
                myDocument.gridPreferences.baselineStart = (newBaselineOffset - itemOffset) + ' px';
            } catch (myException) {
                TMLogger.error('Could not set baseline start (exceeds bounds): ' + (newBaselineOffset - itemOffset) + ' px');
            }
            myDocument.gridPreferences.baselineGridRelativeOption = BaselineGridRelativeOption.TOP_OF_PAGE_OF_BASELINE_GRID_RELATIVE_OPTION;
        } catch (myException) {
            TMLogger.exception("TMExporter.configureBaselineGridSettings", myException + ' (tm_exporter:1415)');
        }
    },
    restoreBaselineGridSettings: function (myPageItem) {
        try {
            var myDocument = myPageItem.parentPage.parent.parent;
            myDocument.gridPreferences.baselineStart = this.savedBaselineStart;
            myDocument.gridPreferences.baselineGridRelativeOption = this.savedBaselineRelative;
        } catch (myException) {
            TMLogger.exception("TMExporter.restoreBaselineGridSettings", myException + ' (tm_exporter:1425)');
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
File.prototype.tmOrientation = function () {
    var baseName = this.name.replace('.indd', '').replace('.indt', '').replace('.idml', '');;
    if (baseName.match("_Pt$") == '_Pt') {
        return 'portrait';
    } else {
        return 'landscape';
    }
};
File.prototype.tmArticleName = function () {
    return TMFiles.getBaseName(this.fullName, true);
};
File.prototype.tmFileExtension = function () {
    var myFileExt = '';
    var myFileNameParts = this.name.split('.');
    if (myFileNameParts.length > 1) {
        myFileExt = myFileNameParts[myFileNameParts.length - 1];
    }
    return myFileExt.toLowerCase();
};
File.prototype.tmReplaceFileExtension = function (newExtension) {
    var myFileExt = '';
    var myFileNameParts = this.name.split('.');
    if (myFileNameParts.length > 1) {
        myFileNameParts = myFileNameParts.slice(0, myFileNameParts.length - 1);
    }
    var myFileName = myFileNameParts.join('.') + newExtension;
    return new File(this.parent.fsName + '/' + myFileName);
}
var TMFiles = {
    createCopyForExport: function (path) {
        TMLogger.debug('Creating copy of: ' + path);
        var srcFile = new File(path);
        var dstFile = new File(Folder.temp.fsName + '/' + kTMP_PREFIX + TMUtilities.uuid() + '/' + srcFile.name);
        if (dstFile.exists) {
            dstFile.remove();
        }
        if (!dstFile.parent.exists) {
            dstFile.parent.create();
        }
        srcFile.copy(dstFile);
        return dstFile;
    },
    getResource: function (name, encoding) {
        try {
            var resourceFolder = '~/Library/Application Support/Twixl Publisher Plugin/Resources';
            if (File.fs == 'Windows') {
                resourceFolder = Folder.appData.fsName + '/Twixl Publisher/Resources';
            }
            var resourceFile = new File(resourceFolder + '/' + name);
            if (!resourceFile.exists) {
                return undefined;
            }
            if (encoding == 'binary') {
                return this.readBinaryDataFromFile(resourceFile.fsName);
            } else {
                return this.readUTF8DataFromFile(resourceFile.fsName);
            }
        } catch (myException) {
            TMLogger.exception("getResource.move", myException + ' (tm_files:70)');
            return undefined;
        }
    },
    move: function (srcPath, dstPath) {
        try {
            TMHelper.call('fs/moveFile', {
                fromPath: srcPath,
                toPath: dstPath
            })
        } catch (myException) {
            TMLogger.exception("TMFiles.move", myException + ' (tm_files:79)');
            return undefined;
        }
    },
    creatorVersion: function (myFile) {
        try {
            var f = new File(myFile);
            if (f.open("r") == false) {
                return undefined;
            } else {
                f.encoding = "binary";
                g = f.read(16);
                h = f.read(8);
                j = f.read(1).charCodeAt(0);
                k = f.read(4);
                l = TMFiles.gl(f, j);
                l_m = TMFiles.gl(f, j);
                f.close();
                return parseInt(l.toString() + l_m.toString());
            }
        } catch (myException) {
            TMLogger.exception("TMFiles.creatorVersion", myException + ' (tm_files:113)');
            return undefined;
        }
    },
    creatorDisplayVersion: function (myFile) {
        var version = this.creatorVersion(myFile);
        if (version == 91) {
            return "Adobe InDesign CC 9.1";
        }
        if (version == 90) {
            return "Adobe InDesign CC 9.0";
        }
        if (version == 80) {
            return "Adobe InDesign CS6";
        }
        if (version == 75) {
            return "Adobe InDesign CS5.5";
        }
        if (version == 70) {
            return "Adobe InDesign CS5";
        }
        return "Unknown version";
    },
    gl: function (a, b) {
        var c = a.read(4);
        if (b == 2) {
            return (c.charCodeAt(3)) + (c.charCodeAt(2) << 8) + (c.charCodeAt(1) << 16) + (c.charCodeAt(0) << 24);
        } else {
            return (c.charCodeAt(0)) + (c.charCodeAt(1) << 8) + (c.charCodeAt(2) << 16) + (c.charCodeAt(3) << 24);
        }
    },
    readUTF8DataFromFile: function (myFilePath) {
        try {
            var myFile = new File(myFilePath);
            myFile.encoding = "UTF-8";
            myFile.open("r");
            var data = myFile.read();
            myFile.close();
            return data;
        } catch (myException) {
            TMStackTrace.addToStack("TMFiles.readUTF8DataFromFile", myException);
        }
    },
    readBinaryDataFromFile: function (myFilePath) {
        try {
            var myFile = new File(myFilePath);
            myFile.encoding = "BINARY";
            myFile.open("r");
            var data = myFile.read();
            myFile.close();
            return data;
        } catch (myException) {
            TMStackTrace.addToStack("TMFiles.readBinaryDataFromFile", myException);
        }
    },
    writeUTF8DataToFile: function (myFilePath, myData) {
        try {
            var myFile = new File(myFilePath);
            myFile.encoding = "UTF-8";
            myFile.open("w");
            myFile.write(myData);
            myFile.close();
        } catch (myException) {
            TMStackTrace.addToStack("TMFiles.writeUTF8ToFile", myException);
        }
    },
    writeBinaryDataToFile: function (myFilePath, myData) {
        try {
            var myFile = new File(myFilePath);
            myFile.encoding = "BINARY";
            myFile.open("w");
            myFile.write(myData);
            myFile.close();
        } catch (myException) {
            TMStackTrace.addToStack("TMFiles.writeBinaryDataToFile", myException);
        }
    },
    browseForFolder: function (myMessage, myRootFolder) {
        try {
            var myFolderLocation = new Folder(myRootFolder).selectDlg(
                myMessage
            );
            if (myFolderLocation != undefined) {
                return myFolderLocation.fsName;
            } else {
                return myFolderLocation;
            }
        } catch (myException) {
            TMLogger.exception("TMFiles.browseForFolder", myException + ' (tm_files:208)');
            return undefined;
        }
    },
    saveFile: function (filePath, prompt) {
        try {
            var file = new File(filePath);
            var result = file.saveDlg(prompt);
            if (result) {
                return result.fsName;
            } else {
                return undefined;
            }
        } catch (myException) {
            TMLogger.exception("TMFiles.saveFile", myException + ' (tm_files:223)');
            return undefined;
        }
    },
    getTemplateNames: function (myOrientations, myShowStatusbar) {
        try {
            var myTemplateFolderName = "Templates";
            if (myOrientations == undefined) {
                myTemplateFolderName = "Templates using Alternate Layout";
            }
            if (File.fs == "Macintosh") {
                var myTemplateRootPath = new Folder('/Library/Application Support/Twixl Publisher Plugin/' + myTemplateFolderName);
                var myTemplateUserPath = new Folder('~/Library/Application Support/Twixl Publisher Plugin/' + myTemplateFolderName);
            } else {
                var myTemplateRootPath = new Folder(Folder.appData + '/Twixl Publisher/' + myTemplateFolderName);
                var myTemplateUserPath = new Folder(Folder.myDocuments + '/Twixl Publisher/' + myTemplateFolderName);
            }
            var mySuffix = (myShowStatusbar) ? '_statusbar' : '_nostatusbar';
            var myRootTemplates = this.getTemplateNamesForFolder(myTemplateRootPath, myOrientations, mySuffix);
            var myUserTemplates = this.getTemplateNamesForFolder(myTemplateUserPath, myOrientations, mySuffix);
            var myTemplates = new Array();
            myTemplates = myTemplates.concat(myRootTemplates);
            myTemplates = myTemplates.concat(myUserTemplates);
            myTemplates.sort(function (a, b) {
                return a.toLowerCase().localeCompare(b.toLowerCase());
            }); // TODO: Should not be case sensitive
            return myTemplates;
        } catch (myException) {
            TMLogger.exception("TMFiles.getTemplateNames", myException + ' (tm_files:263)');
            return [];
        }
    },
    getTemplateNamesForFolder: function (myFolder, myOrientations, mySuffix) {
        try {
            var myTemplates = [];
            var myTemplateItems = myFolder.getFiles();
            for (var i in myTemplateItems) {
                try {
                    var myTemplate = myTemplateItems[i];
                    if (myTemplate.name.tmStartsWith('.')) {
                        continue;
                    }
                    if (myTemplate.tmFileExtension().toLowerCase() != 'indt' && myTemplate.tmFileExtension().toLowerCase() != 'idml') {
                        continue;
                    }
                    if (myOrientations == undefined) {
                        myTemplates.push(myTemplate.displayName.replace('.indt', '').replace('.idml', ''));
                    } else {
                        var myBaseName = this.getReportName(myTemplate.name, false);
                        if (myBaseName.tmEndsWith('_statusbar')) {
                            myBaseName = myBaseName.substr(0, myBaseName.length - 10);
                        }
                        if (myBaseName.tmEndsWith('_nostatusbar')) {
                            myBaseName = myBaseName.substr(0, myBaseName.length - 12);
                        }
                        var myTemplatePt1 = new File(myFolder.fsName + '/' + myBaseName + mySuffix + '_Pt.indt');
                        var myTemplateLs1 = new File(myFolder.fsName + '/' + myBaseName + mySuffix + '_Ls.indt');
                        var myTemplatePt2 = new File(myFolder.fsName + '/' + myBaseName + mySuffix + '_Pt.idml');
                        var myTemplateLs2 = new File(myFolder.fsName + '/' + myBaseName + mySuffix + '_Ls.idml');
                        if (myOrientations == 'landscape' && (myTemplateLs1.exists || myTemplateLs2.exists)) {
                            if (TMUtilities.itemInArray(myTemplates, myBaseName) == false) {
                                myTemplates.push(myBaseName);
                            }
                        }
                        if (myOrientations == 'portrait' && (myTemplatePt1.exists || myTemplatePt2.exists)) {
                            if (TMUtilities.itemInArray(myTemplates, myBaseName) == false) {
                                myTemplates.push(myBaseName);
                            }
                        }
                        if (myOrientations == 'portrait+landscape' && ((myTemplatePt1.exists && myTemplateLs1.exists) || (myTemplatePt2.exists && myTemplateLs2.exists))) {
                            if (TMUtilities.itemInArray(myTemplates, myBaseName) == false) {
                                myTemplates.push(myBaseName);
                            }
                        }
                        if (myOrientations == undefined) {
                            if (myTemplateLs.exists) {
                                myTemplates.push(myBaseName + ' (landscape)');
                            }
                            if (myTemplatePt.exists) {
                                myTemplates.push(myBaseName + ' (portrait)');
                            }
                        }
                    }
                } catch (myException) {
                    TMLogger.exception("TMFiles.getTemplateNamesForFolder.myTemplate", myException + ' (tm_files:339)');
                    continue;
                }
            }
            return myTemplates;
        } catch (myException) {
            TMLogger.exception("TMFiles.getTemplateNamesForFolder", myException + ' (tm_files:349)');
            return [];
        }
    },
    filterFileName: function (name) {
        try {
            name = name.replace(/[,\/#!$%&\*;:{}`~¿\?!\']/g, '_');
            name = url_slug(name);
            name = name.replace(/_+/g, '_');
            name = name.replace(/(^_+|_+$)/g, '');
            return name.tmTrim();
        } catch (myException) {
            TMLogger.exception("TMFiles.filterFileName", myException + ' (tm_files:362)');
            return name;
        }
    },
    slugURL: function (url) {
        if (!url.tmStartsWith('tp-pagelink://')) {
            return url;
        }
        url = url.substr('tp-pagelink://'.length);
        var from = "ãàáäâẽèéëêìíïîõòóöôùúüûñçÃÀÿÄÂẼÈÉËÊÌÿÿÎÕÒÓÖÔÙÚÜÛÑÇ·,:;'";
        var to = "aaaaaeeeeeiiiiooooouuuuncAAAAAEEEEEIIIIOOOOOUUUUNC-----";
        for (var i = 0, l = from.length; i < l; i++) {
            url = url.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i));
        }
        url = TMUtilities.decodeURI(url);
        url = url.replace(/[^a-zA-Z0-9-_\.\/ ]/g, '-')
            .replace(/\s+/g, ' ')
            .replace('+', '-')
            .replace('*', '-')
            .replace('=', '-')
            .replace('$', '-')
            .replace('[', '-')
            .replace(']', '-')
            .replace(/["']/g, '-')
            .replace(/-+/g, '-')
            .replace(/(^-+|-+$)/g, '');
        return 'tp-pagelink://' + url;
    },
    getBaseName: function (myName, slugify) {
        try {
            var myBaseName = myName;
            try {
                myBaseName = TMUtilities.decodeURI(myName);
            } catch (myException) { }
            myBaseName = myBaseName.replace('.indd', '')
                .replace('.indt', '')
                .replace('.idml', '')
                .replace('_Ls', '')
                .replace('_Pt', '')
                .replace(/\\/g, '/')
                .replace(/:/g, '/')
                .replace(/.*\//, '')
                .replace(/_processing$/, '');
            return myBaseName.tmTrim();
        } catch (myException) {
            TMStackTrace.addToStack("TMFiles.getBaseName", myException);
        }
    },
    getReportName: function (myName, slugify) {
        try {
            return this.getBaseName(myName, slugify).replace(new RegExp('%20', 'gm'), ' ');
        } catch (myException) {
            TMStackTrace.addToStack("TMFiles.getReportName", myException);
        }
    },
    getVisibleFiles: function () {
        try {
            var myOpenFiles = [];
            var myAppDocuments = TMUtilities.collectionToArray(app.documents);
            var myAppDocumentsCount = myAppDocuments.length;
            for (var i = 0; i < myAppDocumentsCount; i++) {
                var myDocument = myAppDocuments[i];
                try {
                    if (myDocument.visible) {
                        myOpenFiles.push(myDocument.fullName.fsName);
                    }
                } catch (myException) { }
            }
            return myOpenFiles;
        } catch (myException) {
            TMLogger.exception("TMFiles.getVisibleFiles", myException + ' (tm_files:441)');
            return [];
        }
    },
    getVisibleDocuments: function () {
        try {
            var myOpenDocuments = [];
            var myDocuments = TMUtilities.collectionToArray(app.documents);
            var myDocumentsCount = myDocuments.length;
            for (var i = 0; i < myDocumentsCount; i++) {
                var myDocument = myDocuments[i];
                try {
                    if (myDocument.visible) {
                        myOpenDocuments.push(myDocument);
                    }
                } catch (myException) { }
            }
            return myOpenDocuments;
        } catch (myException) {
            TMLogger.exception("TMFiles.getVisibleDocuments", myException + ' (tm_files:462)');
            return [];
        }
    },
    deleteFolderAndContents: function (myFolder) {
        try {
            TMHelper.call('fs/delete', {
                path: myFolder
            });
        } catch (myException) {
            TMLogger.exception("TMFiles.deleteFolderAndContents", myException + ' (tm_files:471)');
        }
    },
    deleteFolderAndContentsAsync: function (myFolder) {
        try {
            TMTaskQueue.addTask(
                undefined, {
                    'action': 'TMDeleteFolder',
                    'path': myFolder,
                }
            );
        } catch (myException) {
            TMLogger.exception("TMFiles.deleteFolderAndContentsAsync", myException + ' (tm_files:485)');
        }
    },
    copyFolderAndContents: function (mySrcFolder, myDstFolder) {
        try {
            TMHelper.call('fs/copyFolder', {
                fromPath: mySrcFolder,
                toPath: myDstFolder
            });
        } catch (myException) {
            TMLogger.exception("TMFiles.copyFolderAndContents", myException + ' (tm_files:493)');
            return [];
        }
    },
    copyFile: function (srcPath, dstPath) {
        try {
            TMHelper.call('fs/copyFile', {
                fromPath: srcPath,
                toPath: dstPath
            });
        } catch (myException) {
            TMLogger.exception("TMFiles.copyFile", myException + ' (tm_files:502)');
        }
    },
    renameRetinaImages: function (myBaseName, myCount, myType) {
        try {
            var myWrongFile = new File(myBaseName + '.' + myType);
            var myCorrectFile = new File(myBaseName + '1' + '.' + myType);
            if (myWrongFile.exists && !myCorrectFile.exists) {
                myWrongFile.rename(myCorrectFile.name);
            }
            var myWrongFile = new File(myBaseName + '@2x' + '.' + myType);
            var myCorrectFile = new File(myBaseName + '1@2x' + '.' + myType);
            if (myWrongFile.exists && !myCorrectFile.exists) {
                myWrongFile.rename(myCorrectFile.name);
            }
            var myWrongFile = new File(myBaseName + '@3x' + '.' + myType);
            var myCorrectFile = new File(myBaseName + '1@3x' + '.' + myType);
            if (myWrongFile.exists && !myCorrectFile.exists) {
                myWrongFile.rename(myCorrectFile.name);
            }
            for (i = 2; i <= myCount; i++) {
                var myWrongFile = new File(myBaseName + '@2x' + i + '.' + myType);
                var myCorrectFile = new File(myBaseName + i + '@2x' + '.' + myType);
                if (myWrongFile.exists && !myCorrectFile.exists) {
                    myWrongFile.rename(myCorrectFile.name);
                }
            }
            for (i = 2; i <= myCount; i++) {
                var myWrongFile = new File(myBaseName + '@3x' + i + '.' + myType);
                var myCorrectFile = new File(myBaseName + i + '@3x' + '.' + myType);
                if (myWrongFile.exists && !myCorrectFile.exists) {
                    myWrongFile.rename(myCorrectFile.name);
                }
            }
        } catch (myException) {
            TMLogger.exception("TMFiles.renameRetinaImages", myException + ' (tm_files:545)');
            return [];
        }
    },
    getFolderSize: function (folder) {
        try {
            return TMHelper.call('fs/folderSize', {
                path: folder
            }).size;
        } catch (myException) {
            TMLogger.exception("TMFiles.getFolderSize", myException + ' (tm_files:554)');
            return 0;
        }
    },
    removeFileIfExist: function (path) {
        try {
            var file = new File(path);
            if (file.exists) {
                file.remove();
            }
        } catch (myException) {
            TMLogger.exception("TMFiles.removeFileIfExist", myException + ' (tm_files:566)');
        }
    },
    getTextFile: function (path) {
        try {
            var file = new File(path);
            file.open("r");
            var contents = file.read();
            file.close();
            return contents;
        } catch (myException) {
            TMLogger.exception("TMFiles.getTextFile", myException + ' (tm_files:578)');
            return "";
        }
    },
    writeTextFile: function (path, contents) {
        try {
            var file = new File(path);
            file.open("w");
            file.write(contents);
            file.close();
        } catch (myException) {
            TMLogger.exception("TMFiles.writeTextFile", myException + ' (tm_files:590)');
        }
    },
    pathAbsToRel: function (root, filePath) {
        var file = new File(filePath);
        var result = TMUtilities.decodeURI(file.getRelativeURI(root));
        return result;
    },
    pathRelToAbs: function (root, filePath) {
        var file = new File(filePath);
        if (file.exists || filePath.substr(0, 1) == '/') {
            return file.fsName;
        }
        if (root.slice(-1) == '/' || root.slice(-1) == '\\') {
            root = root.slice(0, -1);
        }
        var result = new File(root + '/' + filePath);
        return result.fsName;
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
Folder.prototype.tmSortedFileList = function (pattern) {
    if (!pattern) {
        pattern = '*.*';
    }
    var result = [];
    var files = this.getFiles(pattern);
    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        if (file.name == '.DS_Store') {
            continue;
        }
        if (file.name == '.svn') {
            continue;
        }
        result.push(file.fsName);
    }
    result = TMHelper.call('string/natsort', {
        'array': TMJSON.stringify(result, undefined, '')
    })['array'];
    return result;
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMGeometry = {
    getBounds: function (myBounds) {
        return {
            'x': this.convertToPixels(myBounds[1]),
            'y': this.convertToPixels(myBounds[0]),
            'width': this.convertToPixels(myBounds[3] - myBounds[1]),
            'height': this.convertToPixels(myBounds[2] - myBounds[0]),
        }
    },
    addBounds: function (myObject, myBounds) {
        return TMUtilities.mergeArrays(
            myObject, this.getBounds(myBounds)
        );
    },
    addCorrectedBounds: function (myObject, myBounds) {
        myBounds = {
            'x': this.convertToPixels(myBounds[1]),
            'y': this.convertToPixels(myBounds[0]),
            'width': this.convertToPixels(myBounds[3] - myBounds[1]),
            'height': this.convertToPixels(myBounds[2] - myBounds[0]),
        };
        return TMUtilities.mergeArrays(myObject, myBounds);
    },
    offsetBoundsWithObject: function (myObject, myParentObject) {
        try {
            if (myParentObject == undefined) {
                return myObject;
            }
            if (myParentObject.constructor.name == 'State') {
                myParentObject = myParentObject.parent;
            }
            if (!myParentObject.hasOwnProperty('geometricBounds')) {
                return myObject;
            }
            var myParentBounds = this.getBounds(myParentObject.geometricBounds);
            if (myObject['x'] != undefined) {
                myObject['x'] = myObject['x'] - myParentBounds['x'];
            }
            if (myObject['y'] != undefined) {
                myObject['y'] = myObject['y'] - myParentBounds['y'];
            }
            return myObject;
        } catch (myException) {
            TMLogger.exception("TMGeometry.offsetBoundsWithObject", myException + ' (tm_geometry:53)');
            return myObject;
        }
    },
    offsetBoundsWithXandY: function (myObject, myOffsetX, myOffsetY) {
        try {
            if (myObject['x'] != undefined) {
                myObject['x'] = myObject['x'] + myOffsetX;
            }
            if (myObject['y'] != undefined) {
                myObject['y'] = myObject['y'] + myOffsetY;
            }
            return myObject;
        } catch (myException) {
            TMLogger.exception("TMGeometry.offsetBoundsWithXandY", myException + ' (tm_geometry:68)');
            return myObject;
        }
    },
    offsetObjectsWithXandY: function (myObjects, myOffsetX, myOffsetY) {
        try {
            for (var i = 0; i < myObjects.length; i++) {
                var myObject = myObjects[i];
                if (myObject['x'] != undefined) {
                    myObject['x'] = myObject['x'] + myOffsetX;
                }
                if (myObject['y'] != undefined) {
                    myObject['y'] = myObject['y'] + myOffsetY;
                }
                if (myObject['actions']) {
                    myObject['actions'] = TMGeometry.offsetObjectsWithXandY(myObject['actions'], myOffsetX, myOffsetY);
                }
                myObjects[i] = myObject;
            }
            return myObjects;
        } catch (myException) {
            TMLogger.exception("TMGeometry.offsetObjectsWithXandY", myException + ' (tm_geometry:90)');
            return myObjects;
        }
    },
    mergeBounds: function (myObject, myBounds) {
        var myObject2 = TMUtilities.cloneObject(myObject);
        if (myBounds['x'] > Math.floor(myBounds['x'])) {
            myBounds['x'] = Math.ceil(myBounds['x']);
            myBounds['width'] = myBounds['width'] - 1;
        }
        if (myBounds['y'] > Math.floor(myBounds['y'])) {
            myBounds['y'] = Math.ceil(myBounds['y']);
            myBounds['height'] = myBounds['height'] - 1;
        }
        myBounds['width'] = Math.floor(myBounds['width']);
        myBounds['height'] = Math.floor(myBounds['height']);
        myObject2['x'] = myBounds['x'];
        myObject2['y'] = myBounds['y'];
        myObject2['width'] = myBounds['width'];
        myObject2['height'] = myBounds['height'];
        return myObject2;
    },
    clipToBounds: function (myBounds, myClipBounds) {
        if (myBounds.x < myClipBounds.x) {
            myBounds['width'] = myBounds['width'] - (myClipBounds['x'] - myBounds['x']);
            myBounds['x'] = myClipBounds['x'];
        }
        if (myBounds['y'] < myClipBounds['y']) {
            myBounds['height'] = myBounds['height'] - (myClipBounds['y'] - myBounds['y']);
            myBounds['y'] = myClipBounds['y'];
        }
        if ((myBounds['x'] + myBounds['width']) > myClipBounds['width']) {
            myBounds['width'] = myClipBounds['width'] - myBounds['x'];
        }
        if ((myBounds['y'] + myBounds['height']) > myClipBounds['height']) {
            myBounds['height'] = myClipBounds['height'] - myBounds['y'];
        }
        return myBounds;
    },
    convertToPixels: function (value, resolution) {
        try {
            if (resolution == undefined) {
                resolution = gBaseResolution;
            }
            if (resolution == 72) {
                myVal = UnitValue(value, "px");
                return parseFloat(myVal.as("px").toFixed(kPRECISION));
            } else {
                myVal = UnitValue(value, "mm");
                myVal.baseUnit = UnitValue(1 / resolution, "in");
                return parseFloat(myVal.as("px").toFixed(kPRECISION));
            }
        } catch (myException) {
            TMStackTrace.addToStack("TMGeometry.convertToPixels", myException);
        }
    },
    convertToPoints: function (value, resolution) {
        try {
            if (resolution == undefined) {
                resolution = gBaseResolution;
            }
            if (resolution == 72) {
                myVal = UnitValue(value, "px");
                return myVal.as("pt");
            } else {
                myVal = UnitValue(value, "px");
                myVal.baseUnit = UnitValue(1 / resolution, "in");
                return myVal.as("pt");
            }
        } catch (myException) {
            TMStackTrace.addToStack("TMGeometry.convertToPoints", myException);
        }
    },
    getTextBounds: function (myText) {
        var myBounds = [];
        var mySelOffset = 0.4;
        var myTextLinesCount = myText.lines.length;
        for (var i = 0; i < myTextLinesCount; i++) {
            try {
                var myLine = myText.lines[i];
                var myFirstX = myLine.horizontalOffset - mySelOffset;
                var myLastX = myLine.endHorizontalOffset - mySelOffset;
                var myTopY = myLine.baseline - myLine.ascent - mySelOffset;
                var myBottomY = myLine.endBaseline + myLine.descent + mySelOffset;
                if (i == 0) {
                    myFirstX = myText.horizontalOffset;
                }
                if (i == (myText.lines.length - 1)) {
                    myLastX = myText.endHorizontalOffset - mySelOffset;
                }
                if (myLine.parentTextFrames.length == 0) {
                    continue;
                }
                var parentPage = TMPageItem.parentPage(myLine.parentTextFrames[0]);
                if (!parentPage) {
                    continue;
                }
                var myTextBounds = {
                    x: this.convertToPixels(myFirstX),
                    y: this.convertToPixels(myTopY),
                    width: this.convertToPixels(myLastX - myFirstX),
                    height: this.convertToPixels(myBottomY - myTopY),
                    pageId: parentPage.id,
                };
                myBounds.push(myTextBounds);
            } catch (myException) { }
        }
        if (myBounds.length == 0) {
            return undefined;
        } else {
            return myBounds;
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMHelper = {
    call: function (method, params) {
        var user = $.getenv('USER');
        if (user == undefined || user == "") {
            user = Folder.myDocuments.parent.displayName;
        }
        var url = 'http://127.0.0.1:' + kPORT_HELPER + '/' + method + '?';
        if (params) {
            var formattedParams = [];
            for (var key in params) {
                if (params[key] != undefined) {
                    formattedParams.push(encodeURIComponent(key) + '=' + encodeURIComponent(params[key]));
                } else {
                    formattedParams.push(encodeURIComponent(key) + '=');
                }
            };
            url += formattedParams.join('&');
            url += "&"
        }
        url += "user=" + encodeURIComponent(user);
        url += "&"
        url += "twixl_version=" + encodeURIComponent(TMVersion.version);
        if (method != 'status/queue') {
            TMLogger.debug('Calling URL: ' + url);
        }
        var result;
        if (method == 'string/natsort') {
            var payload = '';
            if (params) {
                var formattedParams = [];
                for (var key in params) {
                    if (params[key] != undefined) {
                        formattedParams.push(encodeURIComponent(key) + '=' + encodeURIComponent(params[key]));
                    } else {
                        formattedParams.push(encodeURIComponent(key) + '=');
                    }
                };
                payload += formattedParams.join('&');
                payload += "&"
            }
            url = 'http://127.0.0.1:' + kPORT_HELPER + '/' + method + '?';
            url += "user=" + encodeURIComponent(user);
            url += "&"
            url += "twixl_version=" + encodeURIComponent(TMVersion.version);
            result = $http({
                method: 'POST',
                payload: payload,
                url: url,
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                forcejson: false
            });
            result = result.payload;
        } else {
            result = TMURL.get(url);
        }
        if (!result) {
            return '';
        }
        TMLogger.debug('Result: ' + result);
        var parsedResult = TMJSON.parse(result);
        if (parsedResult.hasOwnProperty('error')) {
            TMLogger.error(parsedResult['error']);
            TMStackTrace.addToStack('TMHelper.call', parsedResult['error']);
            return;
        }
        return parsedResult;
    },
    previewDevices: function () {
        return this.call('viewers/all');
    },
    isRunning: function () {
        var socket = new Socket();
        if (!socket.open('127.0.0.1:' + kPORT_HELPER, "BINARY")) {
            return false;
        }
        socket.close();
        return true;
    },
    startHelper: function () {
        if (File.fs != "Windows") {
            return;
        }
        TMLogger.info("Twixl Publisher Helper app is not running, starting");
        var helperPath = Folder.system.fsName.split(":\\")[0] + ":\\Program Files (x86)\\Twixl Publisher\\TwixlPublisherHelper.exe";
        TMLogger.info("Helper Path: " + helperPath);
        var helperFile = new File(helperPath);
        helperFile.execute();
    },
};
var $http = (function () {
    return function (config) {
        var url = (/^(.*):\/\/([A-Za-z0-9\-\.]+):?([0-9]+)?(.*)$/).exec(config.url);
        if (url == null) {
            throw "unable to parse URL";
        }
        url = {
            scheme: url[1],
            host: url[2],
            port: url[3] || (url[1] == "https" ? 443 : 80),
            path: url[4]
        };
        if (url.scheme != "http") {
            throw "non-http url's not supported yet!";
        }
        var s = new Socket();
        if (!s.open(url.host + ':' + url.port, 'binary')) {
            throw 'can\'t connect to ' + url.host + ':' + url.port;
        }
        var method = config.method || 'GET';
        var request = method + ' ' + url.path + " HTTP/1.0\r\nConnection: close\r\nHost: " + url.host;
        var header;
        if (config.payload) {
            if (typeof config.payload === 'object') {
                config.payload = JSON.stringify(config.payload);
                (config.headers = config.headers || {})["Content-Type"] = "application/json";
            }
            (config.headers = config.headers || {})["Content-Length"] = config.payload.length;
        }
        for (header in (config.headers || {})) {
            request += "\r\n" + header + ': ' + config.headers[header];
        }
        s.write(request + "\r\n\r\n");
        if (config.payload) {
            s.write(config.payload);
        }
        var data, response, payload, http = {};
        data = s.read();
        while (!s.eof) {
            data += s.read();
        }
        var response = data.indexOf("\r\n\r\n");
        if (response == -1) {
            throw "No HTTP payload found in the response!";
        }
        payload = data.substr(response + 4);
        response = data.substr(0, response);
        var http = /^HTTP\/([\d\.?]+) (\d+) (.*)\r/.exec(response),
            header;
        if (http == null) {
            throw "No HTTP payload found in the response!";
        }
        http = {
            ver: Number(http[1]),
            status: Number(http[2]),
            statusMessage: http[3],
            headers: {}
        };
        var httpregex = /(.*): (.*)\r/g;
        while (header = httpregex.exec(response)) {
            http.headers[header[1]] = header[2];
        }
        var contenttype = (http.headers["Content-Type"] || http.headers["content-type"] || '').split(";");
        payload = decodeURIComponent(escape(payload));
        contenttype = contenttype[0];
        http.payload = payload;
        return http;
    };
})();
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMHostAPI = {
    disableUserInteraction: function () {
        try {
            TMLogger.info('Disabling user interaction');
            TMEventHandlers.removeEventListener();
            app.scriptPreferences.enableRedraw = false;
            app.scriptPreferences.userInteractionLevel = UserInteractionLevels.NEVER_INTERACT;
            TMColorSettings.configureColorSettings();
        } catch (myException) {
            TMLogger.exception("TMHostAPI.disableUserInteraction", myException + ' (tm_host_api:21)');
        }
    },
    enableUserInteraction: function () {
        try {
            TMLogger.info('Enabling user interaction');
            TMProgressBar.close();
            TMColorSettings.restoreColorSettings();
            app.scriptPreferences.userInteractionLevel = UserInteractionLevels.INTERACT_WITH_ALL;
            app.scriptPreferences.enableRedraw = true;
            TMEventHandlers.registerEventListener();
        } catch (myException) {
            TMLogger.exception("TMHostAPI.enableUserInteraction", myException + ' (tm_host_api:40)');
        }
    },
    pluginVersion: function () {
        return TMVersion.version;
    },
    pluginBuild: function () {
        return TMVersion.build;
    },
    showError: function (message) {
        TMDialogs.error(message);
    },
    showMessage: function (message, title) {
        TMDialogs.message(message, title);
    },
    showConfirm: function (message, title) {
        return TMDialogs.confirm(message, title);
    },
    showNewPublication: function () {
        this._callFunction('TMDialogs.showNewPublication');
    },
    showNewArticle: function () {
        this._callFunction('TMDialogs.showNewArticle');
    },
    showPublicationProperties: function () {
        this._callFunction('TMDialogs.showPublicationProperties');
    },
    showArticleProperties: function () {
        this._callFunction('TMDialogs.showArticleProperties');
    },
    showSharePublication: function () {
        this._callFunction('TMDialogs.showSharePublication');
    },
    showShareArticle: function () {
        this._callFunction('TMDialogs.showShareArticle');
    },
    showPreviewPublication: function () {
        this._callFunction('TMDialogs.showPreviewPublication');
    },
    showPreviewArticle: function () {
        this._callFunction('TMDialogs.showPreviewArticle');
    },
    showExportPublication: function () {
        this._callFunction('TMDialogs.showExportPublication');
    },
    showExportArticles: function () {
        this._callFunction('TMDialogs.showExportAsArticles');
    },
    showExportArticle: function () {
        this._callFunction('TMDialogs.showExportArticle');
    },
    showAlternateLayouts: function () {
        this._callFunction('TMDialogs.showAlternateLayouts');
    },
    showPreferences: function () {
        this._callFunction('TMDialogs.showPreferences');
    },
    showAbout: function () {
        this._callFunction('TMDialogs.showAbout');
    },
    exportSupportLogs: function () {
        this._callFunction('TMSupport.exportSupportLogs');
    },
    _callFunction: function (jsFunctionName) {
        try {
            app.doScript(
                jsFunctionName + '()',
                ScriptLanguage.JAVASCRIPT,
                [],
                UndoModes.ENTIRE_SCRIPT
            );
        } catch (myException) {
            TMDialogs.showException(jsFunctionName, myException);
        }
    },
    getWebResourcesPath: function () {
        if (app.books.length > 0 && app.activeBook) {
            var webResourcesDir = new Folder(app.activeBook.filePath.fsName + '/WebResources');
        } else if (app.documents.length > 0 && app.activeDocument) {
            var webResourcesDir = new Folder(app.activeDocument.filePath.fsName + '/WebResources');
        }
        if (!webResourcesDir.exists) {
            webResourcesDir.create();
        }
        return webResourcesDir;
    },
    showWebResources: function () {
        var webResourcesDir = this.getWebResourcesPath();
        webResourcesDir.execute();
    },
    selectWebResource: function (targetPanel, targetProperty) {
        var webResourcesDir = this.getWebResourcesPath();
        var webResourcesFile = new File(webResourcesDir.fsName);
        var selectedFile = webResourcesFile.openDlg('Select the WebResource');
        if (selectedFile != undefined) {
            var selectedFilePath = selectedFile.fsName;
            if (!selectedFilePath.tmStartsWith(webResourcesDir.fsName)) {
                TMDialogs.error("The path \"" + selectedFilePath + "\" is not in the WebResources folder and cannot be used.\n\nOnly files in the WebResources folder can be selected.");
                return;
            }
            selectedFilePath = decodeURI(selectedFilePath.replace(/\\/g, "/"));
            selectedFilePath = selectedFilePath.replace(decodeURI(webResourcesDir.fsName.replace(/\\/g, "/") + '/'), 'webresource://');
            selectedFilePath = encodeURI(selectedFilePath);
            TMUtilities.dispatchEvent(
                "com.twixlmedia.publisher.indesign.events.propertyUpdated", {
                    "targetPanel": targetPanel,
                    "targetProperty": targetProperty,
                    'targetValue': selectedFilePath
                }
            );
        }
    },
    selectFolder: function (targetPanel, targetProperty, prompt) {
        var webResourcesDir = this.getWebResourcesPath();
        var selectedFile = webResourcesDir.selectDlg(prompt);
        if (selectedFile != undefined) {
            var selectedFilePath = selectedFile.fsName.replace(/\\/g, "/");
            var rootFolder = app.activeDocument.filePath.fsName.replace(/\\/g, "/");
            var relativePath = TMUtilities.decodeURI(selectedFile.getRelativeURI(rootFolder));
            TMUtilities.dispatchEvent(
                "com.twixlmedia.publisher.indesign.events.propertyUpdated", {
                    "targetPanel": targetPanel,
                    "targetProperty": targetProperty,
                    'targetValue': relativePath
                }
            );
        }
    },
    selectColor: function (currentValue, targetPanel, targetProperty) {
        var currentColor = parseInt(currentValue, 16);
        var result = $.colorPicker(currentColor);
        TMUtilities.dispatchEvent(
            "com.twixlmedia.publisher.indesign.events.propertyUpdated", {
                "targetPanel": targetPanel,
                "targetProperty": targetProperty,
                'targetValue': result
            }
        );
    },
    logVisibleFiles: function () {
        TMLogger.visibleFiles();
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMJSON;
if (!TMJSON) {
    TMJSON = {};
}
(function () {
    "use strict";

    function f(n) {
        return n < 10 ? '0' + n : n;
    }
    if (typeof Date.prototype.toJSON !== 'function') {
        Date.prototype.toJSON = function (key) {
            return isFinite(this.valueOf()) ?
                this.getUTCFullYear() + '-' +
                f(this.getUTCMonth() + 1) + '-' +
                f(this.getUTCDate()) + 'T' +
                f(this.getUTCHours()) + ':' +
                f(this.getUTCMinutes()) + ':' +
                f(this.getUTCSeconds()) + 'Z' : null;
        };
        String.prototype.toJSON =
            Number.prototype.toJSON =
            Boolean.prototype.toJSON = function (key) {
                return this.valueOf();
            };
    }
    var cx = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
        escapable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
        gap,
        indent,
        meta = { // table of character substitutions
            '\b': '\\b',
            '\t': '\\t',
            '\n': '\\n',
            '\f': '\\f',
            '\r': '\\r',
            '"': '\\"',
            '\\': '\\\\'
        },
        rep;

    function quote(string) {
        escapable.lastIndex = 0;
        return escapable.test(string) ? '"' + string.replace(escapable, function (a) {
            var c = meta[a];
            return typeof c === 'string' ? c :
                '\\u' + ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
        }) + '"' : '"' + string + '"';
    }

    function str(key, holder) {
        var i, // The loop counter.
            k, // The member key.
            v, // The member value.
            length,
            mind = gap,
            partial,
            value = holder[key];
        if (value && typeof value === 'object' &&
            typeof value.toJSON === 'function') {
            value = value.toJSON(key);
        }
        if (typeof rep === 'function') {
            value = rep.call(holder, key, value);
        }
        switch (typeof value) {
            case 'string':
                return quote(value);
            case 'number':
                return isFinite(value) ? String(value) : 'null';
            case 'boolean':
            case 'null':
                return String(value);
            case 'object':
                if (!value) {
                    return 'null';
                }
                gap += indent;
                partial = [];
                if (Object.prototype.toString.apply(value) === '[object Array]') {
                    length = value.length;
                    for (i = 0; i < length; i += 1) {
                        partial[i] = str(i, value) || 'null';
                    }
                    v = partial.length === 0 ? '[]' : gap ?
                        '[\n' + gap + partial.join(',\n' + gap) + '\n' + mind + ']' :
                        '[' + partial.join(',') + ']';
                    gap = mind;
                    return v;
                }
                if (rep && typeof rep === 'object') {
                    length = rep.length;
                    for (i = 0; i < length; i += 1) {
                        if (typeof rep[i] === 'string') {
                            k = rep[i];
                            v = str(k, value);
                            if (v) {
                                partial.push(quote(k) + (gap ? ': ' : ':') + v);
                            }
                        }
                    }
                } else {
                    for (k in value) {
                        if (Object.prototype.hasOwnProperty.call(value, k)) {
                            v = str(k, value);
                            if (v) {
                                partial.push(quote(k) + (gap ? ': ' : ':') + v);
                            }
                        }
                    }
                }
                v = partial.length === 0 ? '{}' : gap ?
                    '{\n' + gap + partial.join(',\n' + gap) + '\n' + mind + '}' :
                    '{' + partial.join(',') + '}';
                gap = mind;
                return v;
        }
    }
    if (typeof TMJSON.stringify !== 'function') {
        TMJSON.stringify = function (value, replacer, space) {
            var i;
            gap = '';
            indent = '';
            if (typeof space === 'number') {
                for (i = 0; i < space; i += 1) {
                    indent += ' ';
                }
            } else if (typeof space === 'string') {
                indent = space;
            } else {
                indent = '    ';
            }
            rep = replacer;
            if (replacer && typeof replacer !== 'function' &&
                (typeof replacer !== 'object' ||
                    typeof replacer.length !== 'number')) {
                throw 'TMJSON.stringify';
            }
            return str('', {
                '': value
            });
        };
    }
    if (typeof TMJSON.parse !== 'function') {
        TMJSON.parse = function (text, reviver) {
            var j;

            function walk(holder, key) {
                var k, v, value = holder[key];
                if (value && typeof value === 'object') {
                    for (k in value) {
                        if (Object.prototype.hasOwnProperty.call(value, k)) {
                            v = walk(value, k);
                            if (v !== undefined) {
                                value[k] = v;
                            } else {
                                delete value[k];
                            }
                        }
                    }
                }
                return reviver.call(holder, key, value);
            }
            text = String(text);
            cx.lastIndex = 0;
            if (cx.test(text)) {
                text = text.replace(cx, function (a) {
                    return '\\u' +
                        ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
                });
            }
            if (/^[\],:{}\s]*$/
                .test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, '@')
                    .replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']')
                    .replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) {
                j = eval('(' + text + ')');
                return typeof reviver === 'function' ?
                    walk({
                        '': j
                    }, '') : j;
            }
            throw new SyntaxError('TMJSON.parse: ' + text);
        };
    }
}());
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMLogger = {
    visibleFiles: function () {
        $.gc();
        TMLogger.info('Number of open files: ' + app.documents.count());
        if (app.documents.count > 100) {
            for (var j = 0; j < app.documents.count(); j++) {
                var myDocument = app.documents.item(j);
                TMLogger.info("    " + (j + 1) + " > " + myDocument.name + " | " + myDocument.tmArticleName());
            }
        }
        $.gc();
    },
    separator: function (message) {
        if (message) {
            this.info(message + '================================================================================');
        } else {
            this.info('================================================================================');
        }
    },
    dump: function (object, title) {
        var myMessage = '';
        if (title != undefined) {
            myMessage += title + "\n";
        }
        myMessage += TMJSON.stringify(object);
        this.debug(myMessage);
    },
    debug: function (message) {
        if (TMPreferences.isDebug()) {
            this.logMessage('DEBUG', message);
        }
    },
    info: function (message) {
        this.logMessage('INFO ', message);
    },
    warn: function (message) {
        this.logMessage('WARN ', message);
    },
    error: function (message) {
        this.logMessage('ERROR', message);
    },
    silentException: function (functionName, exception) {
        this.logMessage('ERROR', functionName + ': ' + exception);
    },
    exception: function (functionName, exception) {
        this.logMessage('ERROR', functionName + ': ' + exception);
    },
    getLogPath: function () {
        if (Folder.fs == "Windows") {
            return Folder.myDocuments.fsName + '/Twixl Publisher/Logs/Twixl Publisher Plugin.log';
        } else {
            return Folder.userData.fsName + '/../Logs/Twixl Publisher/Twixl Publisher Plugin.log';
        }
    },
    openInDefaultApp: function () {
        var logFile = new File(this.getLogPath());
        logFile.execute();
    },
    getFormattedDate: function () {
        var date = new Date();
        var year = date.getFullYear();
        var month = ('00' + (date.getMonth() + 1)).substr(-2);
        var day = ('00' + date.getDate()).substr(-2);
        var hours = ('00' + date.getHours()).substr(-2);
        var minutes = ('00' + date.getMinutes()).substr(-2);
        var seconds = ('00' + date.getSeconds()).substr(-2);
        var millsec = ('000' + date.getMilliseconds()).substr(-3);
        if (TMPreferences.isDebug()) {
            return year + '-' + month + '-' + day + ' ' + hours + ':' + minutes + ':' + seconds + '.' + millsec;
        } else {
            return year + '-' + month + '-' + day + ' ' + hours + ':' + minutes + ':' + seconds;
        }
    },
    logMessage: function (level, message) {
        var logMessage = this.getFormattedDate() + ' | ' + level + ' | ' + message;
        var logFile = new File(this.getLogPath());
        if (!logFile.parent.exists) {
            logFile.parent.create();
        }
        logFile.encoding = "UTF-8";
        if (!logFile.exists || logFile.length > 1024 * 1024 * 5) {
            logFile.open('w');
        } else {
            logFile.open('a');
        }
        logFile.write(logMessage);
        logFile.write('\n');
        logFile.close();
        if (TMApplication.isServer()) {
            if (level == 'ERROR') {
                app.consoleerr(logMessage.substring(this.getFormattedDate().length + level.length + 6));
            } else {
                app.consoleout(logMessage.substring(this.getFormattedDate().length + level.length + 6));
            }
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMObject = {
    writePropertiesToObject: function (objectData) {
        try {
            TMLogger.debug('writePropertiesToObject: ' + objectData.toSource());
            var objectTypes = {
                'slideshow': TMEventHandlers.KEY_MSO,
                'web_viewer': TMEventHandlers.KEY_WV,
                'web_overlay': TMEventHandlers.KEY_WO,
                'scrollable_content': TMEventHandlers.KEY_SC,
                'image': TMEventHandlers.KEY_IMAGE,
                'movie': TMEventHandlers.KEY_MOVIE,
                'sound': TMEventHandlers.KEY_SOUND,
                'image_sequence': TMEventHandlers.KEY_IMAGESEQUENCE,
                'panorama': TMEventHandlers.KEY_PANORAMA,
                'widget': TMEventHandlers.KEY_WIDGET,
            };
            var selection = TMApplication.appSelection();
            if (selection) {
                var clazz = selection.constructor.name;
                if (objectData['panel'] == 'image') {
                    if (clazz == 'Rectangle' && selection.graphics.length == 1) {
                        selection = selection.graphics[0];
                    }
                }
                if (objectData['panel'].tmStartsWith('widget_')) {
                    var widget = 'com.twixlmedia.widget.' + objectData['panel'].substr(7);
                    var properties = objectData['data'];
                    objectData = {
                        'data': {
                            'widget': widget,
                            'properties': properties
                        },
                        'panel': 'widget'
                    };
                }
                if (objectData['panel'] == 'sound') {
                    if (clazz == 'Rectangle') {
                        selection = selection.sounds.firstItem();
                    }
                    selection.playOnPageTurn = TMUtilities.bool(objectData['data']['soundAutoStart']);
                    selection.soundLoop = TMUtilities.bool(objectData['data']['soundLoop']);
                    delete objectData['data']['soundAutoStart'];
                    delete objectData['data']['soundLoop'];
                }
                if (objectData['panel'] == 'movie') {
                    if (clazz == 'Rectangle') {
                        selection = selection.movies.firstItem();
                    }
                    selection.playOnPageTurn = TMUtilities.bool(objectData['data']['movieAutoStart']);
                    selection.movieLoop = TMUtilities.bool(objectData['data']['movieLoop']);
                    selection.controllerSkin = (TMUtilities.bool(objectData['data']['movieController']) ? '' : 'SkinOverAll');
                }
                if (objectData['panel'] == 'slideshow') {
                    selection.name = objectData['data']['msoAnalyticsName'];
                    delete objectData['data']['msoAnalyticsName'];
                }
                var labelKey = objectTypes[objectData['panel']];
                var labelData = TMJSON.stringify(objectData['data']);
                if (labelKey) {
                    selection.insertLabel(labelKey, labelData);
                } else {
                    TMLogger.debug('Not setting label, key is missing');
                }
            } else {
                TMLogger.debug('no selection');
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMObject.writePropertiesToObject', myException);
        }
    },
    assignTypeToObject: function (type) {
        TMLogger.debug('Assigning type to object: ' + type);
        var data = {
            'panel': type,
            'data': {}
        };
        if (type == 'image') {
            data['data']['imageAllowFullScreen'] = 'true';
        }
        TMObject.writePropertiesToObject(data);
        TMEventHandlers.eventSelectionChanged(type, undefined, TMApplication.appSelection());
    },
    unassignObjectType: function () {
        TMLogger.debug('Unassigning type from object');
        var selection = TMApplication.appSelection();
        if (selection) {
            selection.insertLabel(TMEventHandlers.KEY_MSO, '');
            selection.insertLabel(TMEventHandlers.KEY_WV, '');
            selection.insertLabel(TMEventHandlers.KEY_WO, '');
            selection.insertLabel(TMEventHandlers.KEY_SC, '');
            selection.insertLabel(TMEventHandlers.KEY_IMAGE, '');
            selection.insertLabel(TMEventHandlers.KEY_MOVIE, '');
            selection.insertLabel(TMEventHandlers.KEY_SOUND, '');
            selection.insertLabel(TMEventHandlers.KEY_IMAGESEQUENCE, '');
            selection.insertLabel(TMEventHandlers.KEY_PANORAMA, '');
            selection.insertLabel(TMEventHandlers.KEY_WIDGET, '');
            if (selection.graphics.length == 1) {
                selection = selection.graphics[0];
                selection.insertLabel(TMEventHandlers.KEY_IMAGE, '');
            }
        }
        TMEventHandlers.eventSelectionChanged('assign_element', undefined, TMApplication.appSelection());
    },
}
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMPage = {
    tmRelativePageNumber: {},
    tmPageItems: {},
    tmDocumentIntent: {},
    tmOrientation: {},
    tmScreenWidthPx: {},
    tmScreenHeightPx: {},
    tmPageWidthPx: {},
    tmPageHeightPx: {},
    tmPageBaseResolution: {},
    tmUsesAlternateLayouts: {},
    tmPageBounds: {},
    tmLayoutName: {},
    resetCache: function () {
        this.tmRelativePageNumber = {};
        this.tmPageItems = {};
        this.tmDocumentIntent = {};
        this.tmOrientation = {};
        this.tmScreenWidthPx = {};
        this.tmScreenHeightPx = {};
        this.tmPageWidthPx = {};
        this.tmPageHeightPx = {};
        this.tmPageBaseResolution = {};
        this.tmUsesAlternateLayouts = {};
        this.tmPageBounds = {};
        this.tmLayoutName = {};
    },
};
Page.prototype.tmRelativePageNumber = function () {
    try {
        if (this.tmUsesAlternateLayouts()) {
            var pageNumber = 1;
            var mySectionPages = this.appliedSection.tmPages();
            var mySectionPageCount = mySectionPages.length;
            for (var k = 0; k < mySectionPageCount; k++) {
                var myPage = mySectionPages[k];
                if (myPage.id == this.id) {
                    pageNumber = (k + 1);
                    break;
                }
            }
            return pageNumber;
        } else {
            TMPage.tmRelativePageNumber[this.id] = 1;
            var documentPages = TMUtilities.collectionToArray(this.tmParentDocument().pages);
            for (var i = 0; i < documentPages.length; i++) {
                var page = documentPages[i];
                if (page.id == this.id) {
                    TMPage.tmRelativePageNumber[this.id] = (i + 1);
                    break;
                }
            }
        }
        return TMPage.tmRelativePageNumber[this.id];
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmRelativePageNumber", myException + ' (tm_page:71)');
        return undefined;
    }
};
Page.prototype.tmDocumentIntent = function () {
    try {
        return this.tmParentDocument().tmDocumentIntent();
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmDocumentIntent", myException + ' (tm_page:80)');
        return 'web';
    }
}
Page.prototype.tmUsesAlternateLayouts = function () {
    try {
        if (TMPreferences.isRunningLegacyMode() == false) {
            return true;
        }
        if (TMPage.tmUsesAlternateLayouts[this.id] == undefined) {
            TMPage.tmUsesAlternateLayouts[this.id] = this.tmParentDocument().tmUsesAlternateLayouts();
        }
        return TMPage.tmUsesAlternateLayouts[this.id];
    } catch (myException) {
        TMLogger.silentException("Page.prototype.tmUsesAlternateLayouts", myException);
        return false;
    }
};
Page.prototype.tmShowStatusBar = function () {
    try {
        if (app.books.length == 0) {
            return true;
        }
        return app.activeBook.tmShowStatusBar();
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmShowStatusBar", myException + ' (tm_page:107)');
        return true;
    }
};
Page.prototype.tmLayoutName = function () {
    try {
        if (TMPage.tmLayoutName[this.id] == undefined) {
            var usesAlternateLayouts = this.tmUsesAlternateLayouts();
            if (usesAlternateLayouts && this.appliedAlternateLayout) {
                TMPage.tmLayoutName[this.id] = this.appliedAlternateLayout.alternateLayout;
            } else if (usesAlternateLayouts && this.appliedSection) {
                TMPage.tmLayoutName[this.id] = this.appliedSection.alternateLayout;
            } else {
                if (this.tmPageWidthPx == 1024) {
                    TMPage.tmLayoutName[this.id] = 'iPad H';
                } else {
                    TMPage.tmLayoutName[this.id] = 'iPad V';
                }
            }
        }
        return TMPage.tmLayoutName[this.id];
    } catch (myException) {
        return '';
    }
};
Page.prototype.tmLayoutDimensions = function () {
    try {
        var myLayoutName = this.tmLayoutName();
        if (myLayoutName.tmStartsWith('Android 10" ')) {
            return '1280x800';
        } else if (myLayoutName.tmStartsWith('Kindle Fire/Nook ')) {
            return '1024x600';
        } else if (myLayoutName.tmStartsWith('Phone ')) {
            return '568x320';
        } else {
            return '1024x768';
        }
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmLayoutDimensions", myException + ' (tm_page:148)');
        return '1024x768';
    }
};
Page.prototype.tmLayoutPageCount = function () {
    try {
        if (this.tmUsesAlternateLayouts()) {
            return this.appliedAlternateLayout.alternateLayoutLength;
        } else {
            return this.tmParentDocument().pages.count();
        }
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmLayoutPageCount", myException + ' (tm_page:161)');
        return '';
    }
};
Page.prototype.tmOrientation = function () {
    try {
        if (this.tmUsesAlternateLayouts()) {
            var layoutName = this.tmLayoutName();
            if (layoutName.tmEndsWith(' H')) {
                return 'landscape';
            } else if (layoutName.tmEndsWith(' V')) {
                return 'portrait';
            } else {
                var pageWidthPx = this.tmPageWidthPx();
                var pageHeightPx = this.tmPageHeightPx();
                if (pageWidthPx > pageHeightPx) {
                    return 'landscape';
                } else {
                    return 'portrait';
                }
            }
        } else {
            return this.tmParentDocument().tmOrientation();
        }
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmOrientation", myException + ' (tm_page:187)');
        return undefined;
    }
};
Page.prototype.tmRequiredResolution = function () {
    try {
        if (this.tmUsesAlternateLayouts()) {
            var myLayoutName = this.tmLayoutName();
            if (myLayoutName == 'Phone H' || myLayoutName == 'Phone V') {
                return 280;
            }
            if (myLayoutName != 'iPad H' && myLayoutName != 'iPad V') {
                return 72;
            }
        }
        return 144;
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmRequiredResolution", myException + ' (tm_page:205)');
        return 144;
    }
};
Page.prototype.tmScreenWidthPx = function () {
    try {
        if (this.tmUsesAlternateLayouts()) {
            var myLayoutName = this.tmLayoutName();
            if (myLayoutName == 'iPad H') {
                return 1024;
            } else if (myLayoutName == 'iPad V') {
                return 768;
            } else if (myLayoutName == 'Android 10" H') {
                return 1280;
            } else if (myLayoutName == 'Android 10" V') {
                return 800;
            } else if (myLayoutName == 'Kindle Fire/Nook H') {
                return 1024;
            } else if (myLayoutName == 'Kindle Fire/Nook V') {
                return 600;
            } else if (myLayoutName == 'Phone H') {
                return 568;
            } else if (myLayoutName == 'Phone V') {
                return 320;
            }
        }
        var myOrientation = this.tmOrientation();
        if (myOrientation == 'landscape') {
            return 1024;
        } else {
            return 768;
        }
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmScreenWidthPx", myException + ' (tm_page:242)');
        return undefined;
    }
};
Page.prototype.tmScreenHeightPx = function () {
    try {
        if (this.tmUsesAlternateLayouts()) {
            var myLayoutName = this.tmLayoutName();
            if (myLayoutName == 'iPad H') {
                return 768;
            } else if (myLayoutName == 'iPad V') {
                return 1024;
            } else if (myLayoutName == 'Android 10" H') {
                return 752;
            } else if (myLayoutName == 'Android 10" V') {
                return 1232;
            } else if (myLayoutName == 'Kindle Fire/Nook H') {
                return 552;
            } else if (myLayoutName == 'Kindle Fire/Nook V') {
                return 976;
            } else if (myLayoutName == 'Phone H') {
                return 320;
            } else if (myLayoutName == 'Phone V') {
                return 568;
            }
        }
        var myOrientation = this.tmOrientation();
        if (myOrientation == 'landscape') {
            return 768;
        } else {
            return 1024;
        }
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmScreenHeightPx", myException + ' (tm_page:279)');
        return undefined;
    }
};
Page.prototype.tmPageBaseResolution = function () {
    try {
        return this.tmParentDocument().tmDocumentBaseResolution();
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmPageBaseResolution", myException + ' (tm_page:288)');
        return 72;
    }
};
Page.prototype.tmPageWidthPx = function () {
    try {
        var myBounds = this.bounds;
        return Math.round(TMGeometry.convertToPixels(myBounds[3] - myBounds[1], this.tmPageBaseResolution()));
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmPageWidthPx", myException + ' (tm_page:298)');
        return undefined;
    }
};
Page.prototype.tmPageHeightPx = function () {
    try {
        var myBounds = this.bounds;
        return Math.round(TMGeometry.convertToPixels(myBounds[2] - myBounds[0], this.tmPageBaseResolution()));
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmPageHeightPx", myException + ' (tm_page:308)');
        return undefined;
    }
};
Page.prototype.tmParentDocument = function () {
    try {
        return this.parent.parent;
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmParentDocument", myException + ' (tm_page:317)');
        return undefined;
    }
};
Page.prototype.tmPageItems = function () {
    try {
        var pageItems = this.allPageItems.slice(0);
        var layerCount = this.tmParentDocument().layers.length;
        if (layerCount == 1) {
            return pageItems;
        }
        var pageItemsCount = pageItems.length;
        var sortedPageItems = new Array();
        for (var i = 0; i < layerCount; i++) {
            for (var j = 0; j < pageItemsCount; j++) {
                var pageItem = pageItems[j];
                try {
                    if (pageItem == undefined || !pageItem.isValid) {
                        continue;
                    }
                    if (pageItem.itemLayer && pageItem.itemLayer.index == i) {
                        sortedPageItems.push(pageItem);
                    }
                } catch (e) { }
            }
        }
        return sortedPageItems;
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmPageItems", myException + ' (tm_page:351)');
        return [];
    }
};
Page.prototype.tmPageBounds = function () {
    try {
        return TMGeometry.getBounds(this.bounds);
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmPageBounds", myException + ' (tm_page:360)');
        return undefined;
    }
};
Page.prototype.tmPrepareForExport = function (isExport) {
    try {
        if (isExport) {
            var masterPages = [];
            var master = this.appliedMaster;
            try {
                while (master) {
                    masterPages.unshift(master);
                    master = master.appliedMaster;
                }
            } catch (myException) {
                TMLogger.exception("Failed to get list of master page items", myException + ' (tm_page:378)');
                pageItems = pageItems.concat(this.masterPageItems.slice(0));
            }
            TMLogger.info(masterPages);
            for (var i = 0; i < masterPages.length; i++) {
                var masterPage = masterPages[i];
                var pageItems = masterPage.pageItems.everyItem().getElements();
                for (var j = 0; j < pageItems.length; j++) {
                    var pageItem = pageItems[j];
                    try {
                        pageItem.override(this).detach();
                    } catch (myException) { }
                }
            }
        }
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmPrepareForExport", myException + ' (tm_page:398)');
    }
};
Page.prototype.tmItemsByClass = function (myParent, myClass) {
    try {
        var myItems = []
        var myPageItems = this.tmPageItems();
        var myCount = myPageItems.length;
        for (var i = 0; i < myCount; i++) {
            var myItem = myPageItems[i];
            try {
                if (myItem.hasOwnProperty('visible') && myItem.visible == false) {
                    continue;
                }
            } catch (myException) { }
            if (myItem.constructor.name == myClass) {
                if (myParent != undefined && TMPageItem.isChildOf(myItem, myParent)) {
                    myItems.push(myItem);
                }
                if (myParent == undefined && !TMPageItem.isNested(myItem)) {
                    myItems.push(myItem);
                }
            }
        }
        return myItems;
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmItemsByClass", myException + ' (tm_page:430)');
        return [];
    }
};
Page.prototype.tmAddBehaviors = function (myCollection, myBehaviors) {
    try {
        myBehaviors = TMUtilities.collectionToArray(myBehaviors);
        var myCount = myBehaviors.length;
        for (var i = 0; i < myCount; i++) {
            var myBehavior = myBehaviors[i];
            var myPageItem = myBehavior.parent;
            if (myBehavior.enableBehavior && myBehavior.tmProperties(this) != undefined) {
                myCollection.push(myBehavior.tmProperties(this));
            }
        }
        return myCollection;
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmAddBehaviors", myException + ' (tm_page:448)');
        return [];
    }
};
Page.prototype.tmActions = function (myTmpPublication, myArticlePathRel, myArticlePathFull, myParent) {
    try {
        var myButtons = [];
        var myItems = this.tmItemsByClass(myParent, 'Button');
        for (var i in myItems) {
            var myItem = myItems[i];
            var myButton = {};
            myButton = TMGeometry.addCorrectedBounds(myButton, myItem.visibleBounds);
            var myActions = [];
            myActions = this.tmAddBehaviors(myActions, myItem.gotoStateBehaviors);
            myActions = this.tmAddBehaviors(myActions, myItem.gotoPreviousStateBehaviors);
            myActions = this.tmAddBehaviors(myActions, myItem.gotoNextStateBehaviors);
            myActions = this.tmAddBehaviors(myActions, myItem.movieBehaviors);
            myActions = this.tmAddBehaviors(myActions, myItem.soundBehaviors);
            myActions = this.tmAddBehaviors(myActions, myItem.gotoAnchorBehaviors);
            myActions = this.tmAddBehaviors(myActions, myItem.gotoPageBehaviors);
            myActions = this.tmAddBehaviors(myActions, myItem.gotoPreviousPageBehaviors);
            myActions = this.tmAddBehaviors(myActions, myItem.gotoNextPageBehaviors);
            myActions = this.tmAddBehaviors(myActions, myItem.gotoFirstPageBehaviors);
            myActions = this.tmAddBehaviors(myActions, myItem.gotoLastPageBehaviors);
            myActions = this.tmAddBehaviors(myActions, myItem.gotoURLBehaviors);
            myButton['actions'] = myActions;
            try {
                myButton['id'] = myItem.id;
            } catch (myException) {
                TMLogger.exception("Page.prototype.tmActions", myException + ' (tm_page:485)');
            }
            var myStates = TMUtilities.collectionToArray(myItem.states);
            var myStateNames = [];
            for (var i = 0; i < myStates.length; i++) {
                myStateNames.push(myStates[i].statetype);
            }
            if (TMUtilities.itemInArray(myStateNames, StateTypes.DOWN)) {
                for (var j in myStates) {
                    var myState = myStates[j];
                    var myStateType = myState.statetype;
                    if (myStateType != StateTypes.UP && myStateType != StateTypes.DOWN) {
                        continue;
                    }
                    var myStateName = (myStateType == StateTypes.UP) ? 'normal' : 'click';
                    TMProgressBar.updateSubLabel("Button: " + myItem.id);
                    var myFileFormat = (kEXPORT_FORMAT == 'PDF') ? '.pdf' : '.png';
                    var myButtonPathRel = myArticlePathRel + '_b' + myItem.id + '_' + myStateName + myFileFormat;
                    var myButtonPathFull = myArticlePathFull + '_b' + myItem.id + '_' + myStateName;
                    myItem.activeStateIndex = parseInt(j);
                    if (myParent) {
                        if (myParent.constructor.name == 'State') {
                            myParentGeometry = myParent.parent.visibleBounds;
                        } else {
                            myParentGeometry = myParent.visibleBounds;
                        }
                        myOffsetX = myItem.visibleBounds[1] - myParentGeometry[1];
                        myOffsetY = myItem.visibleBounds[0] - myParentGeometry[0];
                    } else {
                        myOffsetX = 0;
                        myOffsetY = 0;
                    }
                    if (this.tmLayoutDimensions() == '1024x768' && (!TMExporter.exportDeviceType || !TMExporter.exportDeviceType.tmStartsWith('phone_'))) {
                        TMExporter.exportTransparentObject(this, myItem, myButtonPathFull + '@2x' + myFileFormat, 0, 0, 0, 0, gBaseResolution * 2, 1.0, 1.0, false, kEXPORT_FORMAT);
                    }
                    if (this.tmLayoutDimensions() == '1280x800' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('android'))) {
                        TMExporter.exportTransparentObject(this, myItem, myButtonPathFull + myFileFormat, 0, 0, 0, 0, gBaseResolution, 1.0, 1.0, false, kEXPORT_FORMAT);
                    }
                    if (this.tmLayoutDimensions() == '1024x600' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('android'))) {
                        TMExporter.exportTransparentObject(this, myItem, myButtonPathFull + myFileFormat, 0, 0, 0, 0, gBaseResolution, 1.0, 1.0, false, kEXPORT_FORMAT);
                    }
                    if (this.tmLayoutDimensions() == '568x320' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('phone_'))) {
                        TMExporter.exportTransparentObject(this, myItem, myButtonPathFull.replace('568x320', '736x414') + '@3x' + myFileFormat, 0, 0, 0, 0, gBaseResolution * (2208 / 568), 1.0, 1.0, false, kEXPORT_FORMAT);
                    }
                    myButton[myStateName] = myButtonPathRel;
                }
            } else {
                if (myActions.length == 0) {
                    continue;
                }
            }
            myButtons.push(myButton);
        }
        for (var i in myButtons) {
            var myButton = myButtons[i];
            myButton = TMGeometry.offsetBoundsWithObject(myButton, myParent);
            for (var j in myButton['actions']) {
                myButton['actions'][j] = TMGeometry.offsetBoundsWithObject(myButton['actions'][j], myParent);
            }
            myButtons[i] = myButton;
        }
        return myButtons.reverse();
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmActions", myException + ' (tm_page:572)');
        return [];
    }
};
Page.prototype.tmPageLinks = function (myParent) {
    try {
        var myPageLinks = [];
        var myHyperlinks = this.tmHyperlinks(myParent);
        for (var i in myHyperlinks) {
            var myHyperlink = myHyperlinks[i];
            if (myHyperlink['linkType'] == 'pagelinks') {
                delete myHyperlink['linkType'];
                delete myHyperlink['pageId'];
                myPageLinks.push(myHyperlink);
            }
        }
        for (var i in myPageLinks) {
            var myPageLink = myPageLinks[i];
            myPageLinks[i] = TMGeometry.offsetBoundsWithObject(myPageLink, myParent);
        }
        return myPageLinks.reverse();
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmPageLinks", myException + ' (tm_page:600)');
        return [];
    }
};
Page.prototype.tmWebLinks = function (myParent) {
    try {
        var myWebLinks = [];
        var myHyperlinks = this.tmHyperlinks(myParent);
        for (var i in myHyperlinks) {
            var myHyperlink = myHyperlinks[i];
            if (myHyperlink['linkType'] == 'weblinks') {
                delete myHyperlink['linkType'];
                delete myHyperlink['pageId'];
                delete myHyperlink['action'];
                myWebLinks.push(myHyperlink);
            }
        }
        for (var i in myWebLinks) {
            var myWebLink = myWebLinks[i];
            myWebLinks[i] = TMGeometry.offsetBoundsWithObject(myWebLink, myParent);
        }
        return myWebLinks.reverse();
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmWebLinks", myException + ' (tm_page:629)');
        return [];
    }
};
Page.prototype.tmPageTextBounds = function () {
    try {
        var myDocument = this.tmParentDocument();
        var myPageWidth = myDocument.documentPreferences.pageWidth;
        var myPageHeight = myDocument.documentPreferences.pageHeight;
        if (this.side == PageSideOptions.leftHand) {
            var myX2 = this.marginPreferences.left;
            var myX1 = this.marginPreferences.right;
        } else {
            var myX1 = this.marginPreferences.left;
            var myX2 = this.marginPreferences.right;
        }
        var myY1 = this.marginPreferences.top;
        var myX2 = myPageWidth - myX2;
        var myY2 = myPageHeight - this.marginPreferences.bottom;
        return [myY1, myX1, myY2, myX2];
    } catch (myException) {
        TMStackTrace.addToStack("Page.prototype.tmPageTextBounds", myException);
    }
};
Page.prototype.tmTextContents = function () {
    try {
        var myContents = "";
        var myPageItems = this.tmPageItems();
        var myPageItemsCnt = myPageItems.length;
        for (var i = 0; i < myPageItems.length; i++) {
            var myPageItem = myPageItems[i];
            if (myPageItem.constructor.name == 'TextFrame') {
                myContents = myContents + myPageItem.contents + " ";
                var myTables = TMUtilities.collectionToArray(myPageItem.tables);
                for (var j = 0; j < myTables.length; j++) {
                    var myTable = myTables[j];
                    var myTableCells = TMUtilities.collectionToArray(myTable.cells);
                    for (var k = 0; k < myTableCells.length; k++) {
                        var myCell = myTableCells[k];
                        myContents = myContents + myCell.contents + " ";
                    }
                }
            }
            if (myPageItem.constructor.name == 'MultiStateObject') {
                var myStates = TMUtilities.collectionToArray(myPageItem.states);
                var myStatesCount = myStates.length;
                if (myStatesCount > 1) {
                    for (var j = 1; j < myStatesCount; j++) {
                        var myState = myStates[j];
                        var myStateItems = TMUtilities.collectionToArray(myState.pageItems);
                        var myStateItemsCnt = myStateItems.length;
                        for (var k = 0; k < myStateItemsCnt; k++) {
                            var myStateItem = myStateItems[k];
                            if (myStateItem.constructor.name == 'TextFrame') {
                                myContents = myContents + myStateItem.contents + " ";
                                var myTables = TMUtilities.collectionToArray(myStateItem.tables);
                                for (var l = 0; l < myTables.length; l++) {
                                    var myTable = myTables[l];
                                    var myTableCells = TMUtilities.collectionToArray(myTable.cells);
                                    for (var k = 0; k < myTableCells.length; k++) {
                                        var myCell = myTableCells[k];
                                        myContents = myContents + myCell.contents + " ";
                                    }
                                }
                            }
                            if (myStateItem.constructor.name == 'Group') {
                                var myStateGroupItems = myStateItem.allPageItems;
                                var myStateGroupItemsCnt = myStateGroupItems.length;
                                for (var l = 0; l < myStateGroupItemsCnt; l++) {
                                    var myStateGroupItem = myStateGroupItems[l];
                                    if (myStateGroupItem.constructor.name == 'TextFrame') {
                                        myContents = myContents + myStateGroupItem.contents + " ";
                                        var myTables = TMUtilities.collectionToArray(myStateGroupItem.tables);
                                        for (var m = 0; m < myTables.length; m++) {
                                            var myTable = myTables[m];
                                            var myTableCells = TMUtilities.collectionToArray(myTable.cells);
                                            for (var n = 0; n < myTableCells.length; n++) {
                                                var myCell = myTableCells[n];
                                                myContents = myContents + myCell.contents + " ";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        myContents = myContents.replace(/\u0018/g, this.name);
        myContents = myContents.replace(/(\r\n|\n|\r|\t)/gm, ' ');
        myContents = myContents.replace(/\s+/g, ' ');
        myContents = myContents.replace(/\s+$/g, '');
        myContents = myContents.tmTrim().toLocaleLowerCase();
        return myContents;
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmTextContents", myException + ' (tm_page:742)');
        return "";
    }
};
Page.prototype.tmHyperlinks = function (myParent) {
    try {
        var myHyperlinks = [];
        var myPageId = this.id;
        var myDocumentHyperlinks = this.tmParentDocument().tmHyperlinks(this, myParent);
        for (var i in myDocumentHyperlinks) {
            var myHyperlink = myDocumentHyperlinks[i];
            delete myHyperlink['action'];
            myHyperlinks.push(myHyperlink);
        }
        return myHyperlinks.reverse();
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmHyperlinks", myException + ' (tm_page:763)');
        return [];
    }
};
Page.prototype.tmMovies = function (myParent, myTmpPublication) {
    try {
        var myMovies = [];
        if (myParent && myParent.constructor.name == 'State') {
            var myItems = TMPageItem.getPageItemChildren(myParent);
        } else {
            var myItems = this.tmPageItems();
        }
        var myCount = myItems.length;
        for (var i = 0; i < myCount; i++) {
            var myPageItem = myItems[i];
            if (myPageItem instanceof Movie) {
                if (myParent == undefined && TMPageItem.isNested(myPageItem)) {
                    continue;
                }
                if (myParent != undefined && !TMPageItem.isChildOf(myPageItem, myParent)) {
                    continue;
                }
                var myProps = null;
                var myLabel = myPageItem.extractLabel("com.rovingbird.epublisher.movie");
                if (myLabel != '') {
                    myProps = TMJSON.parse(myLabel);
                }
                var myDestFileName = undefined;
                try {
                    myDestFileName = myPageItem.url;
                    if (!myDestFileName.tmStartsWith('http://') && !myDestFileName.tmStartsWith('https://')) {
                        throw 'Not a valid movie url: ' + myDestFileName;
                    }
                } catch (myException) {
                    var mySrcFile = new File(myPageItem.filePath);
                    if (mySrcFile.exists) {
                        var mySrcChecksum = TMHelper.call('fs/checksum', {
                            path: mySrcFile.fsName
                        }).checksum
                        if (myProps && TMUtilities.getBoolProperty(myProps, 'movieStreaming', false)) {
                            mySrcChecksum += '_streaming';
                        }
                        if (!TMCache.isMediaLinkCached(mySrcChecksum)) {
                            myDestFileName = 'MediaResources/movie_' + mySrcChecksum + '.mp4';
                            var myDestFile = myTmpPublication + '/' + myDestFileName;
                            if (!mySrcFile.copy(myDestFile)) {
                                TMLogger.error('Failed to copy movie: ' + mySrcFile);
                                myDestFileName = undefined;
                            } else {
                                TMLogger.info('Copied movie: ' + mySrcFile);
                            }
                            TMCache.cacheMediaLink(mySrcChecksum, myDestFileName);
                        } else {
                            myDestFileName = TMCache.getCachedMediaLink(mySrcChecksum);
                        }
                    }
                }
                if (myDestFileName != undefined) {
                    var myShowControls = true;
                    if (myPageItem.controllerSkin != '') {
                        myShowControls = (myPageItem.showController == true);
                    }
                    var myMovieData = {
                        id: myPageItem.id,
                        movie: myDestFileName,
                        auto: (myPageItem.playOnPageTurn == true) ? "yes" : "no",
                        controls: (myShowControls == true) ? "yes" : "no",
                        loop: myPageItem.movieLoop ? "yes" : "no",
                        fullScreen: TMUtilities.getBoolProperty(myProps, 'movieShowFullScreen', 'no'),
                        returnToPosterFrame: TMUtilities.getBoolProperty(myProps, 'movieReturnToPosterFrame', 'no'),
                        analyticsName: TMUtilities.getProperty(myProps, 'movieAnalyticsName', "" + myPageItem.id),
                        streaming: TMUtilities.getBoolProperty(myProps, 'movieStreaming', false),
                        delay: TMUtilities.getProperty(myProps, 'movieDelay', '0.0'),
                    };
                    if (myMovieData['fullScreen'] == 'yes') {
                        myMovieData['controls'] = 'yes';
                    }
                    if (myMovieData['auto'] == 'no') {
                        myMovieData['delay'] = 0;
                    }
                    if (myPageItem.parent.constructor.name == 'Character') {
                        myMovieData = TMGeometry.addCorrectedBounds(myMovieData, myPageItem.geometricBounds);
                    } else {
                        myMovieData = TMGeometry.addCorrectedBounds(myMovieData, myPageItem.parent.geometricBounds);
                    }
                    myMovieData = TMGeometry.offsetBoundsWithObject(myMovieData, myParent);
                    myMovies.push(myMovieData);
                    TMLogger.info('    Exported: ' + myDestFileName);
                } else {
                    TMLogger.info('    Skipping Movie (not found): ' + myPageItem.filePath.fsName);
                }
            }
        }
        return myMovies.reverse();
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmMovies", myException + ' (tm_page:896)');
        return [];
    }
};
Page.prototype.tmSounds = function (myParent, myTmpPublication) {
    try {
        var mySounds = [];
        if (myParent && myParent.constructor.name == 'State') {
            var myItems = TMPageItem.getPageItemChildren(myParent);
        } else {
            var myItems = this.tmPageItems();
        }
        var myCount = myItems.length;
        for (var i = 0; i < myCount; i++) {
            var myPageItem = myItems[i];
            if (myPageItem instanceof Sound) {
                if (myParent == undefined && TMPageItem.isNested(myPageItem)) {
                    continue;
                }
                if (myParent != undefined && !TMPageItem.isChildOf(myPageItem, myParent)) {
                    continue;
                }
                var mySrcFile = new File(myPageItem.filePath);
                var myDestFileName = 'MediaResources/sound_' + TMUtilities.uuid() + ".mp3";
                var myDestFile = myTmpPublication + '/' + myDestFileName;
                if (mySrcFile.exists) {
                    var mySrcChecksum = TMHelper.call('fs/checksum', {
                        path: mySrcFile.fsName
                    }).checksum
                    if (!TMCache.isMediaLinkCached(mySrcChecksum)) {
                        if (!mySrcFile.copy(myDestFile)) {
                            TMLogger.error('Failed to copy movie: ' + mySrcFile);
                        } else {
                            TMLogger.info('Copied sound: ' + mySrcFile);
                        }
                        TMCache.cacheMediaLink(mySrcChecksum, myDestFileName);
                    } else {
                        myDestFileName = TMCache.getCachedMediaLink(mySrcChecksum);
                    }
                    var myProps = null;
                    var myLabel = myPageItem.extractLabel("com.rovingbird.epublisher.sound");
                    if (myLabel != '') {
                        myProps = TMJSON.parse(myLabel);
                    }
                    var mySoundData = {
                        id: myPageItem.id,
                        movie: myDestFileName,
                        auto: myPageItem.playOnPageTurn ? "yes" : "no",
                        loop: myPageItem.soundLoop ? "yes" : "no",
                        analyticsName: TMUtilities.getProperty(myProps, 'soundAnalyticsName', "" + myPageItem.id),
                        delay: TMUtilities.getProperty(myProps, 'soundDelay', '0.0'),
                    }
                    if (mySoundData['auto'] == 'no') {
                        mySoundData['delay'] = 0;
                    }
                    mySoundData = TMGeometry.addCorrectedBounds(mySoundData, myPageItem.parent.geometricBounds);
                    mySoundData = TMGeometry.offsetBoundsWithObject(mySoundData, myParent);
                    mySounds.push(mySoundData);
                    TMLogger.info('    Exported: ' + myDestFileName);
                } else {
                    TMLogger.info('    Skipping Sound (not found): ' + myPageItem.filePath.fsName);
                }
            }
        }
        return mySounds.reverse();
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmSounds", myException + ' (tm_page:996)');
        return [];
    }
};
Page.prototype.tmWebViewers = function (myRootFolderName, myArticlePathRel, myParent) {
    try {
        var myWebViewers = [];
        if (myParent && myParent.constructor.name == 'State') {
            var myItems = TMPageItem.getPageItemChildren(myParent);
        } else {
            var myItems = this.tmPageItems();
        }
        var myCount = myItems.length;
        for (var i = 0; i < myCount; i++) {
            var myPageItem = myItems[i];
            var myLabel = myPageItem.extractLabel("com.rovingbird.epublisher.wv");
            if (myLabel != '') {
                var myProps = TMJSON.parse(myLabel);
                if (myProps.hasOwnProperty('wvUrl') && myProps.wvUrl != "") {
                    var url = TMUtilities.normalizeUrl(myProps.wvUrl);
                    if (url == undefined) {
                        continue;
                    }
                    if (myParent == undefined && TMPageItem.isNested(myPageItem)) {
                        continue;
                    }
                    if (myParent != undefined && !TMPageItem.isChildOf(myPageItem, myParent)) {
                        continue;
                    }
                    var myWebViewer = {
                        id: myPageItem.id,
                        url: url.tmTrim(),
                        userInteractionAllowed: myProps.wvAllowUserInteraction ? "yes" : "no",
                        transparent: myProps.wvTransparent ? "yes" : "no",
                        showScrollbars: myProps.wvShowScrollbars ? "yes" : "no",
                        openLinksInline: myProps.wvOpenLinksInline ? "yes" : "no",
                        showLoadingIndicator: myProps.wvShowLoadingIndicator ? "yes" : "no",
                    };
                    myWebViewer = TMGeometry.addCorrectedBounds(myWebViewer, myPageItem.geometricBounds);
                    myWebViewer = TMGeometry.offsetBoundsWithObject(myWebViewer, myParent);
                    myWebViewers.push(myWebViewer);
                }
            }
        }
        var myPanoramas = this.tmPanoramas(myRootFolderName, myParent);
        myWebViewers = myWebViewers.concat(myPanoramas);
        var myWidgets = this.tmWidgets(myRootFolderName, myParent);
        myWebViewers = myWebViewers.concat(myWidgets);
        var myHtmlItems = this.tmHtmlItems(myRootFolderName, myArticlePathRel, myParent);
        myWebViewers = myWebViewers.concat(myHtmlItems);
        return myWebViewers.reverse();
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmWebViewers", myException + ' (tm_page:1079)');
        return [];
    }
};
Page.prototype.tmHtmlItems = function (myRootFolderName, myArticlePathRel, myParent) {
    try {
        var myHtmlItems = [];
        if (myParent && myParent.constructor.name == 'State') {
            var myItems = TMPageItem.getPageItemChildren(myParent);
        } else {
            var myItems = this.tmPageItems();
        }
        for (var i = 0; i < myItems.length; i++) {
            var myPageItem = myItems[i];
            if (myPageItem.constructor.name != 'HtmlItem') {
                continue;
            }
            if (myParent == undefined && TMPageItem.isNested(myPageItem)) {
                continue;
            }
            if (myParent != undefined && !TMPageItem.isChildOf(myPageItem, myParent)) {
                continue;
            }
            var url = undefined;
            var srcPath = TMDocument.linkedFiles[myPageItem.id];
            if (srcPath) {
                var srcFile = new File(srcPath);
                if (!srcFile.exists) {
                    continue;
                }
                var webresourcesPath = new Folder(myRootFolderName + '/WebResources');
                var dstExt = srcFile.tmFileExtension();
                if (dstExt.toLowerCase() != 'oam') {
                    continue;
                }
                var dstName = TMCrypto.MD5(srcFile.fsName) + '.' + dstExt;
                var dstPath = myRootFolderName + '/WebResources/' + dstName;
                var dstFile = new File(dstPath);
                var url = 'webresource://' + dstName
                if (!dstFile.exists) {
                    TMFiles.copyFile(srcPath, dstPath);
                }
            }
            if (myPageItem.htmlContent) {
                var dstName = TMCrypto.MD5(myArticlePathRel + '_htmlitem_' + myPageItem.id) + '.html';
                var dstPath = myRootFolderName + '/WebResources/' + dstName;
                var url = 'webresource://' + dstName;
                var htmlData = '<!DOCTYPE html><html><head><title></title><style type="text/css" media="screen">body { margin: 0px; padding: 0px; }</style></head><body>' + myPageItem.htmlContent + '</body></html>';
                TMFiles.writeUTF8DataToFile(dstPath, htmlData);
            }
            if (url) {
                var myWebViewer = {
                    id: myPageItem.id,
                    url: url,
                    userInteractionAllowed: "yes",
                    transparent: "yes",
                    showScrollbars: "no",
                    openLinksInline: "yes",
                    showLoadingIndicator: "no",
                };
                myWebViewer = TMGeometry.addCorrectedBounds(myWebViewer, myPageItem.parent.geometricBounds);
                myWebViewer = TMGeometry.offsetBoundsWithObject(myWebViewer, myParent);
                myHtmlItems.push(myWebViewer);
            }
        }
        return myHtmlItems;
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmHtmlItems", myException + ' (tm_page:1171)');
        return [];
    }
};
Page.prototype.tmWidgets = function (myRootFolderName, myParent) {
    try {
        var myWidgets = [];
        if (File.fs == "Macintosh") {
            var widgetFolder = "~/Library/Application Support/Twixl Publisher Plugin/Widgets/";
        } else {
            var widgetFolder = Folder.appData.fsName + "\\Twixl Publisher\\Widgets\\";
        }
        if (!TMExporter.exportID) {
            TMExporter.exportID = TMUtilities.uuid();
        }
        var widgetUUID = TMExporter.exportID;
        if (myParent && myParent.constructor.name == 'State') {
            var myItems = TMPageItem.getPageItemChildren(myParent);
        } else {
            var myItems = this.tmPageItems();
        }
        var myCount = myItems.length;
        for (var i = 0; i < myCount; i++) {
            var myPageItem = myItems[i];
            var myLabel = myPageItem.extractLabel("com.rovingbird.epublisher.widget");
            if (myLabel != '') {
                var myProps = TMJSON.parse(myLabel);
                if (myProps.hasOwnProperty('widget') && myProps.widget != "") {
                    var widgetSrcPath = new Folder(widgetFolder + myProps.widget);
                    if (!widgetSrcPath.exists) {
                        continue;
                    }
                    if (myProps.widget == 'com.twixlmedia.widget.rawhtml') {
                        if (myProps.hasOwnProperty('properties') && (!myProps['properties'].hasOwnProperty('raw_html') || myProps['properties']['raw_html'] == '')) {
                            continue;
                        }
                    }
                    if (myParent == undefined && TMPageItem.isNested(myPageItem)) {
                        continue;
                    }
                    if (myParent != undefined && !TMPageItem.isChildOf(myPageItem, myParent)) {
                        continue;
                    }
                    var myWebResources = new Folder(myRootFolderName + "/WebResources/" + widgetUUID);
                    if (!myWebResources.exists) {
                        myWebResources.create();
                    }
                    var widgetSrcJsLib = new File(widgetFolder + "/twixl_widget.js");
                    var widgetDstJsLib = new File(myWebResources.fsName + "/twixl_widget.js");
                    if (!widgetDstJsLib.exists) {
                        widgetSrcJsLib.copy(widgetDstJsLib);
                        TMLogger.info("    Exported: " + widgetSrcJsLib.name);
                    }
                    var widgetDstPath = new Folder(myWebResources.fsName + "/" + myProps.widget);
                    if (!widgetDstPath.exists) {
                        TMFiles.copyFolderAndContents(widgetSrcPath.fsName, widgetDstPath.fsName);
                        TMLogger.info("    Exported: " + myProps.widget);
                    }
                    TMFiles.removeFileIfExist(widgetDstPath.fsName + "/manifest.json");
                    var myWebViewer = {
                        id: myPageItem.id,
                        url: "",
                        userInteractionAllowed: "yes",
                        transparent: "yes",
                        showScrollbars: "no",
                        openLinksInline: "no",
                        showLoadingIndicator: "yes",
                    };
                    myWebViewer = TMGeometry.addCorrectedBounds(myWebViewer, myPageItem.geometricBounds);
                    myWebViewer = TMGeometry.offsetBoundsWithObject(myWebViewer, myParent);
                    myProps.properties["width"] = myWebViewer["width"];
                    myProps.properties["height"] = myWebViewer["height"];
                    var widgetDescription = TMFiles.getTextFile(widgetSrcPath.fsName + "/manifest.json");
                    var widgetProperties = TMJSON.parse(widgetDescription);
                    if (widgetProperties.hasOwnProperty("inject_parameters")) {
                        var widgetHtml = TMFiles.getTextFile(widgetSrcPath.fsName + "/widget.html");
                        var widgetUrlParams = {};
                        var parameterCount = widgetProperties.parameters.length;
                        for (var j = 0; j < parameterCount; j++) {
                            var parameterDesc = widgetProperties.parameters[j];
                            if (parameterDesc instanceof Object && parameterDesc.hasOwnProperty("name")) {
                                var parameterName = parameterDesc['name'];
                                var parameterValue = "";
                                if (parameterDesc.hasOwnProperty("default")) {
                                    parameterValue = parameterDesc["default"].toString();
                                }
                                if (myProps.hasOwnProperty("properties") && myProps.properties.hasOwnProperty(parameterName)) {
                                    parameterValue = myProps.properties[parameterName].toString();
                                }
                                if (parameterDesc.hasOwnProperty("inject_parameter") && parameterDesc["inject_parameter"] == true) {
                                    if (parameterName != 'raw_html') {
                                        parameterValue = parameterValue.replace(/\r\n/g, " ").replace(/\n/g, " ").replace(/\r/g, " ");
                                        parameterValue = parameterValue.replace("'", "\\'");
                                    }
                                    widgetHtml = widgetHtml.replace('{twixl_widget_var:' + parameterName + '}', parameterValue);
                                } else {
                                    widgetUrlParams[parameterName] = parameterValue;
                                }
                                if (parameterName != 'raw_html') {
                                    myWebViewer[parameterName] = parameterValue;
                                }
                            }
                        }
                        myWebViewer['widget'] = myProps.widget;
                        widgetHtml = widgetHtml.replace('{twixl_widget_var:width}', myWebViewer["width"]);
                        widgetHtml = widgetHtml.replace('{twixl_widget_var:height}', myWebViewer["height"]);
                        var widgetFileName = TMUtilities.uuid() + ".html";
                        TMFiles.writeTextFile(widgetDstPath.fsName + "/" + widgetFileName, widgetHtml);
                        var widgetUrl = "webresource://" + widgetUUID + "/" + myProps.widget + "/" + widgetFileName;
                        widgetUrl += TMUtilities.urlSerialize(widgetUrlParams);
                        myWebViewer["url"] = widgetUrl;
                        TMFiles.removeFileIfExist(widgetDstPath.fsName + "/widget.html");
                    } else {
                        var widgetUrl = "webresource://" + widgetUUID + "/" + myProps.widget + "/widget.html";
                        widgetUrl += TMUtilities.urlSerialize(myProps.properties);
                        myWebViewer["url"] = widgetUrl;
                    }
                    myWidgets.push(myWebViewer);
                }
            }
        }
        return myWidgets;
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmWidgets", myException + ' (tm_page:1376)');
        return [];
    }
};
Page.prototype.tmWebOverlays = function (myParent) {
    try {
        var myWebOverlays = [];
        if (myParent && myParent.constructor.name == 'State') {
            var myItems = TMPageItem.getPageItemChildren(myParent);
        } else {
            var myItems = this.tmPageItems();
        }
        var myCount = myItems.length;
        for (var i = 0; i < myCount; i++) {
            var myPageItem = myItems[i];
            var myLabel = myPageItem.extractLabel("com.rovingbird.epublisher.wo");
            if (myLabel != '') {
                var myProps = TMJSON.parse(myLabel);
                if (myProps.hasOwnProperty('woUrl') && myProps.woUrl != "") {
                    if (myParent == undefined && TMPageItem.isNested(myPageItem)) {
                        continue;
                    }
                    if (myParent != undefined && !TMPageItem.isChildOf(myPageItem, myParent)) {
                        continue;
                    }
                    var myOverlayX = 0;
                    var myOverlayY = 0;
                    var myOverlayWidth = this.tmScreenWidthPx();
                    var myOverlayHeight = this.tmScreenHeightPx();
                    if (myProps.woWidth > 0) {
                        var myOverlayX = Math.round((this.tmScreenWidthPx() - myProps.woWidth) / 2);
                        var myOverlayWidth = myProps.woWidth;
                    }
                    if (myProps.woHeight > 0) {
                        var myOverlayY = Math.round((this.tmScreenHeightPx() - myProps.woHeight) / 2);
                        var myOverlayHeight = myProps.woHeight;
                    }
                    var url = TMUtilities.normalizeUrl(myProps.woUrl);
                    if (url == undefined) {
                        continue;
                    }
                    var myWebOverlay = {
                        id: myPageItem.id,
                        url: url.tmTrim(),
                        xpopup: myOverlayX,
                        ypopup: myOverlayY,
                        wpopup: myOverlayWidth,
                        hpopup: myOverlayHeight,
                        userInteractionAllowed: myProps.woAllowUserInteraction ? "yes" : "no",
                        showScrollbars: myProps.woShowScrollbars ? "yes" : "no",
                        showLoadingIndicator: myProps.woShowLoadingIndicator ? "yes" : "no",
                        backgroundColor: "000000",
                        backgroundOpacity: 0.5,
                        analyticsName: TMUtilities.getProperty(myProps, "woAnalyticsName", "" + myPageItem.id),
                    };
                    try {
                        if (myProps.hasOwnProperty('woBackgroundColor')) {
                            myWebOverlay['backgroundColor'] = TMUtilities.hexColor(myProps.woBackgroundColor.toString(16));
                        }
                    } catch (myException) { }
                    try {
                        if (myProps.hasOwnProperty('woBackgroundOpacity')) {
                            myWebOverlay['backgroundOpacity'] = myProps.woBackgroundOpacity / 100;
                        }
                    } catch (myException) { }
                    myWebOverlay = TMGeometry.addCorrectedBounds(myWebOverlay, myPageItem.visibleBounds);
                    myWebOverlay = TMGeometry.offsetBoundsWithObject(myWebOverlay, myParent);
                    myWebOverlays.push(myWebOverlay);
                }
            }
        }
        return myWebOverlays.reverse();
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmWebOverlays", myException + ' (tm_page:1488)');
        return [];
    }
};
Page.prototype.tmImageSequences = function (myPublicationPath) {
    try {
        var myImageSequences = [];
        var myItems = this.tmPageItems();
        var myCount = myItems.length;
        for (var i = 0; i < myCount; i++) {
            var myPageItem = myItems[i];
            var myLabel = myPageItem.extractLabel("com.rovingbird.epublisher.imagesequence");
            if (myLabel != '') {
                TMLogger.info("Parsing ISQ label: " + myLabel);
                var myProps = TMJSON.parse(myLabel);
                TMLogger.info("Parsed ISQ label: " + myProps.toSource());
                if (myProps.hasOwnProperty('folder') && myProps.folder != "") {
                    var myImageSequenceFolder = undefined;
                    TMLogger.info('Checking ISQ folder 1: ' + myProps.folder)
                    var myImageSequenceFolder1 = new Folder(myProps.folder);
                    if (!myImageSequenceFolder && myImageSequenceFolder1.exists) {
                        TMLogger.info("Image Sequence Folder 1: " + myImageSequenceFolder1);
                        myImageSequenceFolder = myImageSequenceFolder1;
                    }
                    if (myImageSequenceFolder == undefined) {
                        if (app.books.length > 0) {
                            var myImageSequenceFolder2 = new Folder(app.activeBook.filePath.fsName + '/' + myProps.folder);
                            TMLogger.info('Checking ISQ folder 2: ' + app.activeBook.filePath.fsName + '/' + myProps.folder)
                            if (!myImageSequenceFolder && myImageSequenceFolder2.exists) {
                                TMLogger.info("Image Sequence Folder 2: " + myImageSequenceFolder2);
                                myImageSequenceFolder = myImageSequenceFolder2;
                            }
                        }
                    }
                    if (myImageSequenceFolder == undefined) {
                        var myImageSequenceFolder3 = new Folder(this.tmParentDocument().filePath.fsName + '/' + myProps.folder);
                        TMLogger.info('Checking ISQ folder 3: ' + this.tmParentDocument().filePath.fsName + '/' + myProps.folder)
                        if (!myImageSequenceFolder && myImageSequenceFolder3.exists) {
                            TMLogger.info("Image Sequence Folder 3: " + myImageSequenceFolder3);
                            myImageSequenceFolder = myImageSequenceFolder3;
                        }
                    }
                    TMLogger.info("Image Sequence Folder: " + myImageSequenceFolder.fsName);
                    if (myImageSequenceFolder) {
                        TMLogger.info("Image Sequence Folder: " + myImageSequenceFolder.fsName);
                        var myImageSequenceID = TMCrypto.MD5(myImageSequenceFolder.fsName);
                        if (TMCache.isMediaLinkCached(myImageSequenceFolder)) {
                            myImageSequenceID = TMCache.getCachedMediaLink(myImageSequenceFolder);
                        } else {
                            TMCache.cacheMediaLink(myImageSequenceFolder, myImageSequenceID);
                        }
                        var dstPath = new Folder(myPublicationPath + '/MediaResources/' + myImageSequenceID);
                        if (!dstPath.exists) {
                            dstPath.create();
                        }
                        var speed = TMUtilities.getProperty(myProps, 'speed', 16);
                        var interval = 1 / speed;
                        var myImageSequence = {
                            id: myPageItem.id,
                            basepath: 'MediaResources/' + myImageSequenceID + '/isq',
                            format: '',
                            count: 0,
                            reverse: TMUtilities.getBoolProperty(myProps, 'reverse', 'no'),
                            loop: TMUtilities.getBoolProperty(myProps, 'loop', 'yes'),
                            allowSwiping: TMUtilities.getBoolProperty(myProps, 'allow_swiping', 'yes'),
                            tapPlayPause: TMUtilities.getBoolProperty(myProps, 'tap_play_pause', 'no'),
                            autoPlay: TMUtilities.getBoolProperty(myProps, 'auto_play', 'no'),
                            interval: interval,
                            speed: speed,
                            delay: TMUtilities.getProperty(myProps, 'delay', '0'),
                        };
                        myImageSequence = TMGeometry.addCorrectedBounds(myImageSequence, myPageItem.geometricBounds);
                        var myImages = myImageSequenceFolder.tmSortedFileList();
                        for (var myImagesIdx = 0; myImagesIdx < myImages.length; myImagesIdx++) {
                            var myImageFile = myImages[myImagesIdx];
                            var myExt = myImageFile.tmFileExtension().toLowerCase();
                            if (myImageFile.tmContains("@2x.") > 0) {
                                continue;
                            }
                            if (myExt == 'jpg' || myExt == 'jpeg' || myExt == 'png') {
                                var myIdx = ('000' + myImageSequence['count']);
                                myIdx = myIdx.substr(myIdx.length - 3);
                                var myExt = myImageFile.tmFileExtension();
                                var mySourcePath = new File(myImageFile);
                                var myDestPath = new File(myPublicationPath + '/MediaResources/' + myImageSequenceID + '/isq_' + myIdx + "." + myExt);
                                var mySourcePathRetina = new File(myImageFile.replace('.' + myExt, '@2x.' + myExt));
                                var myDestPathRetina = new File(myPublicationPath + '/MediaResources/' + myImageSequenceID + '/isq_' + myIdx + "@2x." + myExt);
                                if (!myDestPath.exists) {
                                    if (!mySourcePath.copy(myDestPath)) {
                                        TMLogger.error("Failed to copy: " + mySourcePath.fsName);
                                    }
                                    if (mySourcePathRetina.exists && TMExporter.exportDeviceType != 'android') {
                                        if (!mySourcePathRetina.copy(myDestPathRetina)) {
                                            TMLogger.error("Failed to copy: " + mySourcePathRetina.fsName);
                                        }
                                    }
                                }
                                myImageSequence['count']++;
                                myImageSequence['format'] = myExt;
                            }
                        }
                        myImageSequences.push(myImageSequence);
                    } else {
                        TMLogger.error("Failed to find ISQ folder: " + myProps.folder);
                    }
                }
            }
        }
        return myImageSequences.reverse();
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmImageSequences", myException + ' (tm_page:1641)');
        return [];
    }
};
Page.prototype.tmPanoramas = function (myRootFolderName, myParent) {
    return [];
};
Page.prototype.tmScrollables = function (myTmpPublication, myArticlePathRel, myArticlePathFull, myOptions) {
    try {
        var myScrollables = [];
        var myItems = this.tmPageItems();
        var myCount = myItems.length;
        for (var i = 0; i < myCount; i++) {
            var myPageItem = myItems[i];
            if (TMPageItem.isScrollable(myPageItem)) {
                var myProps = TMPageItem.scrollableProperties(myPageItem);
                if (myProps.scAllowScrolling && myPageItem.allPageItems.length > 0) {
                    try {
                        while (myPageItem.isValid && myPageItem.parent.isValid && myPageItem.parent instanceof Group) {
                            myPageItem.parent.ungroup();
                        }
                    } catch (myException) { }
                    TMProgressBar.updateSubLabel("Scrollable contents: " + myPageItem.id);
                    var myBaseName = myArticlePathRel + '_sc' + myPageItem.id;
                    var myBasePath = myArticlePathFull + '_sc' + myPageItem.id;
                    var myScrollingBounds = myPageItem.geometricBounds;
                    var myScrollingContent = myPageItem.allPageItems[0];
                    var myScrollingContentBounds = myScrollingContent.visibleBounds;
                    var myScrollData = {};
                    myScrollData['id'] = myPageItem.id;
                    myScrollData['showScrollbars'] = TMUtilities.getBoolProperty(myProps, 'scShowScrollbars', 'yes');
                    myScrollData['enablePaging'] = 'no'; //TMUtilities.getBoolProperty(myProps, 'scEnablePaging', 'no');
                    myScrollData['enableZooming'] = TMUtilities.getBoolProperty(myProps, 'scEnableZooming', 'no');
                    var myFileFormat = ((myScrollData['enableZooming'] == 'yes' || myScrollData['enableZooming'] == true) ? '.jpg' : '.png');
                    if (kEXPORT_FORMAT == 'PDF') {
                        myFileFormat = '.pdf';
                    }
                    myScrollData['analyticsName'] = TMUtilities.getProperty(myProps, "scAnalyticsName", "" + myPageItem.id);
                    myScrollData = TMGeometry.addCorrectedBounds(myScrollData, myScrollingBounds);
                    myScrollData['contentX'] = TMGeometry.convertToPixels(myScrollingContentBounds[1]) - myScrollData['x'];
                    myScrollData['contentY'] = TMGeometry.convertToPixels(myScrollingContentBounds[0]) - myScrollData['y'];
                    myScrollData['contentWidth'] = Math.round(TMGeometry.convertToPixels(myScrollingContentBounds[3] - myScrollingContentBounds[1]));
                    myScrollData['contentHeight'] = Math.round(TMGeometry.convertToPixels(myScrollingContentBounds[2] - myScrollingContentBounds[0]));
                    var offsetX = 0;
                    var offsetY = 0;
                    if (myScrollData['enableZooming'] == 'no' || myScrollData['enableZooming'] == false) {
                        if (myScrollData['contentX'] > 0) {
                            offsetX = myScrollData['contentX'];
                            myScrollData['contentX'] = 0;
                            myScrollData['contentWidth'] += offsetX;
                        }
                        if (myScrollData['contentY'] > 0) {
                            offsetY = myScrollData['contentY'];
                            myScrollData['contentY'] = 0;
                            myScrollData['contentHeight'] += offsetY;
                        }
                    }
                    var paddingX = 0;
                    var paddingY = 0;
                    if (myScrollData['enableZooming'] == 'no' || myScrollData['enableZooming'] == false) {
                        paddingX = myScrollData['width'] - (myScrollData['contentX'] + myScrollData['contentWidth']); // + offsetX;
                        if (paddingX > 0) {
                            myScrollData['contentWidth'] += paddingX;
                        } else {
                            paddingX = 0;
                        }
                        paddingY = myScrollData['height'] - (myScrollData['contentY'] + myScrollData['contentHeight']); //+ offsetY;
                        if (paddingY > 0) {
                            myScrollData['contentHeight'] += paddingY;
                        } else {
                            paddingY = 0;
                        }
                    }
                    var exportFormat = kEXPORT_FORMAT;
                    if (myScrollData['enableZooming']) {
                        exportFormat = 'JPG';
                        myFileFormat = '.jpg';
                    }
                    myScrollData['file'] = myBaseName + myFileFormat;
                    var hScale = parseFloat(1.0);
                    var vScale = parseFloat(1.0);
                    if (this.tmLayoutDimensions() == '1024x768' && (!TMExporter.exportDeviceType || !TMExporter.exportDeviceType.tmStartsWith('phone_'))) {
                        TMExporter.exportTransparentObject(this, myScrollingContent, myBasePath + '@2x' + myFileFormat, offsetX, offsetY, paddingX, paddingY, gBaseResolution * 2, hScale, vScale, false, exportFormat);
                    }
                    if (this.tmLayoutDimensions() == '1280x800' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('android'))) {
                        TMExporter.exportTransparentObject(this, myScrollingContent, myBasePath + myFileFormat, offsetX, offsetY, paddingX, paddingY, gBaseResolution, hScale, vScale, false, exportFormat);
                    }
                    if (this.tmLayoutDimensions() == '1024x600' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('android'))) {
                        TMExporter.exportTransparentObject(this, myScrollingContent, myBasePath + myFileFormat, offsetX, offsetY, paddingX, paddingY, gBaseResolution, hScale, vScale, false, exportFormat);
                    }
                    if (this.tmLayoutDimensions() == '568x320' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('phone_'))) {
                        TMExporter.exportTransparentObject(this, myScrollingContent, myBasePath.replace('568x320', '736x414') + '@3x' + myFileFormat, offsetX, offsetY, paddingX, paddingY, gBaseResolution * (2208 / 568), hScale, vScale, false, exportFormat);
                    }
                    myScrollData['weblinks'] = this.tmWebLinks(myScrollingContent);
                    myScrollData['pagelinks'] = this.tmPageLinks(myScrollingContent);
                    myScrollData['actions'] = this.tmActions(myTmpPublication, myArticlePathRel, myArticlePathFull, myScrollingContent);
                    myScrollData['movies'] = this.tmMovies(myScrollingContent, myTmpPublication);
                    myScrollData['sounds'] = this.tmSounds(myScrollingContent, myTmpPublication);
                    myScrollData['weboverlays'] = this.tmWebOverlays(myScrollingContent);
                    myScrollData['webviewers'] = this.tmWebViewers(myTmpPublication, myArticlePathRel, myScrollingContent);
                    if (offsetX > 0 || offsetY > 0) {
                        myScrollData['weblinks'] = TMGeometry.offsetObjectsWithXandY(myScrollData['weblinks'], offsetX, offsetY);
                        myScrollData['pagelinks'] = TMGeometry.offsetObjectsWithXandY(myScrollData['pagelinks'], offsetX, offsetY);
                        myScrollData['actions'] = TMGeometry.offsetObjectsWithXandY(myScrollData['actions'], offsetX, offsetY);
                        myScrollData['movies'] = TMGeometry.offsetObjectsWithXandY(myScrollData['movies'], offsetX, offsetY);
                        myScrollData['sounds'] = TMGeometry.offsetObjectsWithXandY(myScrollData['sounds'], offsetX, offsetY);
                        myScrollData['weboverlays'] = TMGeometry.offsetObjectsWithXandY(myScrollData['weboverlays'], offsetX, offsetY);
                        myScrollData['webviewers'] = TMGeometry.offsetObjectsWithXandY(myScrollData['webviewers'], offsetX, offsetY);
                    }
                    myScrollables.push(myScrollData);
                }
            }
        }
        return myScrollables.reverse();
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmScrollables", myException + ' (tm_page:1809)');
        return [];
    }
};
Page.prototype.tmFullScreenImages = function (myTmpPublication, myArticlePathRel, myArticlePathFull, myOptions, myOrientation, myEmptyPngName) {
    try {
        var myImages = [];
        var myItems = this.tmPageItems();
        var myCount = myItems.length;
        for (var i = 0; i < myCount; i++) {
            var myPageItem = myItems[i];
            var myLabel = myPageItem.extractLabel("com.rovingbird.epublisher.image");
            if (myLabel != '') {
                var myProps = TMJSON.parse(myLabel);
                try {
                    while (myPageItem.isValid && myPageItem.parent.isValid && myPageItem.parent instanceof Group) {
                        myPageItem.parent.ungroup();
                    }
                } catch (myException) { }
                var myIsInInteractiveItem = false;
                var myParents = TMPageItem.parents(myPageItem);
                var myParentsCount = myParents.length;
                for (var j = 0; j < myParentsCount; j++) {
                    var myParentItem = myParents[j];
                    if (TMPageItem.isScrollable(myParentItem)) {
                        TMLogger.info('Skipping: ' + myPageItem + ' (parent is scrollable area)');
                        myIsInInteractiveItem = true;
                        break;
                    }
                    if (TMPageItem.isSlideShow(myParentItem)) {
                        TMLogger.info('Skipping: ' + myPageItem + ' (parent is slide show)');
                        myIsInInteractiveItem = true;
                        break;
                    }
                }
                if (myIsInInteractiveItem == true) {
                    continue;
                }
                if (myProps.imageAllowFullScreen) {
                    myPageItem = myPageItem.parent;
                    var myBaseName = myArticlePathRel + '_i' + myPageItem.id;
                    var myExpPath = myArticlePathFull + '_i' + myPageItem.id;
                    var myExpPathFull = myArticlePathFull + '_i' + myPageItem.id + '_full';
                    var myFileFormat = (kEXPORT_FORMAT == 'PDF') ? '.pdf' : '.jpg';
                    var fullWidth = this.tmScreenWidthPx();
                    var fullHeight = this.tmScreenHeightPx();
                    var myBounds = myPageItem.visibleBounds;
                    var myImageData = {
                        id: myPageItem.id,
                        showScrollViewIndicator: "no",
                        scrollViewIndicatorOpacity: 0.5,
                        scrollViewIndicatorBackgroundColor: '000000',
                        scrollViewIndicatorActiveColor: 'FFFFFF',
                        scrollViewIndicatorInactiveColor: 'AAAAAA',
                        userInteractionAllowed: "yes",
                        showScrollBars: "no",
                        allowFullScreen: "yes",
                        imageFileFormat: "JPG", // vs: format
                        initialSlide: myPageItem.id,
                        transitionStyle: "none",
                        transitionDuration: "1.0",
                        autoPlay: "no",
                        interval: "3.0",
                        delay: "0.0",
                        tapPlayPause: "no",
                        slides: [],
                        loop: "no",
                        stopAtState: "",
                        analyticsName: TMUtilities.getProperty(myProps, "imageAnalyticsName", "" + myPageItem.id),
                    }
                    myImageData = TMGeometry.addCorrectedBounds(myImageData, myBounds);
                    TMProgressBar.updateSubLabel("Full screen image: " + myImageData['analyticsName']);
                    if (myImageData.width == 0 || myImageData.height == 0 || myImageData.width == undefined || myImageData.height == undefined) {
                        var myException = new Error("Invalid slideshow dimensions: " + myImageData.toSource());
                        TMStackTrace.addToStack("Page.prototype.tmFullScreenImages", myException);
                    }
                    var imageWidth = myImageData.width;
                    var imageHeight = myImageData.height;
                    var scaleX = fullWidth / imageWidth;
                    var scaleY = fullHeight / imageHeight;
                    var scale = (scaleX < scaleY) ? scaleX : scaleY;
                    var offsetX = 0;
                    var offsetY = 0;
                    if (this.tmLayoutDimensions() == '1024x768' && (!TMExporter.exportDeviceType || !TMExporter.exportDeviceType.tmStartsWith('phone_'))) {
                        TMExporter.exportTransparentObject(this, myPageItem, myExpPathFull + '@2x' + myFileFormat, offsetX, offsetY, 0, 0, gBaseResolution * 2, scale, scale, true, kEXPORT_FORMAT);
                    }
                    if (this.tmLayoutDimensions() == '1280x800' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('android'))) {
                        TMExporter.exportTransparentObject(this, myPageItem, myExpPathFull + myFileFormat, offsetX, offsetY, 0, 0, gBaseResolution, scale, scale, true, kEXPORT_FORMAT);
                    }
                    if (this.tmLayoutDimensions() == '1024x600' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('android'))) {
                        TMExporter.exportTransparentObject(this, myPageItem, myExpPathFull + myFileFormat, offsetX, offsetY, 0, 0, gBaseResolution, scale, scale, true, kEXPORT_FORMAT);
                    }
                    if (this.tmLayoutDimensions() == '568x320' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('phone_'))) {
                        TMExporter.exportTransparentObject(this, myPageItem, myExpPathFull.replace('568x320', '736x414') + '@3x' + myFileFormat, offsetX, offsetY, 0, 0, gBaseResolution * (2208 / 568), scale, scale, false, kEXPORT_FORMAT);
                    }
                    var mySlideData = {
                        name: myPageItem.id,
                        file: myEmptyPngName,
                        full: myBaseName + '_full' + myFileFormat,
                        scale: scale,
                    };
                    myImageData['slides'].push(mySlideData);
                    myImages.push(myImageData);
                }
            }
        }
        return myImages;
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmFullScreenImages", myException + ' (tm_page:1965)');
        return [];
    }
};
Page.prototype.tmMultiStateObjects = function (myTmpPublication, myArticlePathRel, myArticlePathFull, myOptions, myOrientation, myEmptyPngName) {
    try {
        var myMultistates = [];
        var myPageMso = this.tmItemsByClass(undefined, 'MultiStateObject');
        var myPageMsoCount = myPageMso.length;
        for (var m = 0; m < myPageMsoCount; m++) {
            var mySlideShow = myPageMso[m];
            mySlideShow.activeStateIndex = 0;
            try {
                while (mySlideShow.parent instanceof Group) {
                    mySlideShow.parent.ungroup();
                }
            } catch (myException) { }
            var myBounds = mySlideShow.visibleBounds;
            var mySlideShowData = {
                id: mySlideShow.id,
                showScrollViewIndicator: "no",
                scrollViewIndicatorOpacity: 0.5,
                scrollViewIndicatorBackgroundColor: '000000',
                scrollViewIndicatorActiveColor: 'FFFFFF',
                scrollViewIndicatorInactiveColor: 'AAAAAA',
                userInteractionAllowed: "yes",
                showScrollBars: "yes",
                allowFullScreen: "no",
                format: "JPG",
                slides: [],
                initialSlide: mySlideShow.states.firstItem().name,
                transitionStyle: 'none',
                transitionDuration: "1.0",
                autoPlay: "no",
                interval: "3.0",
                delay: "0.0",
                tapPlayPause: "no",
                loop: "no",
                stopAtState: "",
                analyticsName: mySlideShow.name,
                fullScreenBackgroundColor: "000000",
            }
            mySlideShowData = TMGeometry.addCorrectedBounds(mySlideShowData, myBounds);
            var offsetX = (mySlideShowData['x'] < 0) ? mySlideShowData['x'] : 0;
            var offsetY = (mySlideShowData['y'] < 0) ? mySlideShowData['y'] : 0;
            mySlideShowData = TMGeometry.clipToBounds(mySlideShowData, this.tmPageBounds());
            if (mySlideShowData.width == 0 || mySlideShowData.height == 0) {
                continue;
            }
            var myLabel = mySlideShow.extractLabel("com.rovingbird.epublisher.mso");
            if (myLabel != '') {
                var myProps = TMJSON.parse(myLabel);
                mySlideShowData['showScrollViewIndicator'] = TMUtilities.getBoolProperty(myProps, 'msoShowScrollViewIndicator', 'no');
                mySlideShowData['scrollViewIndicatorOpacity'] = TMUtilities.getProperty(myProps, 'msoScrollViewIndicatorOpacity', 50) / 100;
                mySlideShowData['scrollViewIndicatorBackgroundColor'] = TMUtilities.getColorProperty(myProps, 'msoScrollViewIndicatorBackgroundColor', '000000');
                mySlideShowData['scrollViewIndicatorActiveColor'] = TMUtilities.getColorProperty(myProps, 'msoScrollViewIndicatorActiveColor', 'FFFFFF');
                mySlideShowData['scrollViewIndicatorInactiveColor'] = TMUtilities.getColorProperty(myProps, 'msoScrollViewIndicatorInactiveColor', 'AAAAAA');
                mySlideShowData['format'] = TMUtilities.getProperty(myProps, 'msoFileFormat', 'JPG');
                mySlideShowData['transitionStyle'] = TMUtilities.getProperty(myProps, 'msoTransitionStyle', 'none').replace(/ /gm, '').toLowerCase();
                mySlideShowData['transitionDuration'] = TMUtilities.getProperty(myProps, 'msoTransitionDuration', '1.0');
                mySlideShowData['autoPlay'] = TMUtilities.getBoolProperty(myProps, 'msoAllowAutoPlay', 'no');
                mySlideShowData['interval'] = TMUtilities.getProperty(myProps, 'msoInterval', '3.0');
                mySlideShowData['delay'] = TMUtilities.getProperty(myProps, 'msoDelay', '0.0');
                mySlideShowData['tapPlayPause'] = TMUtilities.getBoolProperty(myProps, 'msoTapPlayPause', 'no');
                mySlideShowData['loop'] = TMUtilities.getBoolProperty(myProps, 'msoAllowLoop', 'no');
                mySlideShowData['userInteractionAllowed'] = TMUtilities.getBoolProperty(myProps, 'msoAllowUserInteraction', 'yes');
                mySlideShowData['showScrollBars'] = TMUtilities.getBoolProperty(myProps, 'msoShowScrollbars', 'no');
                mySlideShowData['allowFullScreen'] = TMUtilities.getBoolProperty(myProps, 'msoAllowFullScreen', 'no');
                mySlideShowData['fullScreenBackgroundColor'] = TMUtilities.getColorProperty(myProps, 'msoFullScreenBackgroundColor', '000000');
            }
            var msoWidth = mySlideShowData.width;
            var msoHeight = mySlideShowData.height;
            var fullWidth = this.tmScreenWidthPx();
            var fullHeight = this.tmScreenHeightPx();
            var scaleX = fullWidth / msoWidth;
            var scaleY = fullHeight / msoHeight;
            var scale = (scaleX < scaleY) ? scaleX : scaleY;
            var mySlideShowStates = TMUtilities.collectionToArray(mySlideShow.states);
            var mySlideShowStatesCount = mySlideShowStates.length;
            for (var n = 0; n < mySlideShowStatesCount; n++) {
                try {
                    TMProgressBar.updateSubLabel("Slide show: " + mySlideShowData['analyticsName'] + " (slide " + (n + 1) + ' of ' + mySlideShowStatesCount + ")");
                    var myState = mySlideShowStates[n];
                    var myExpPath = new File(myArticlePathFull + '_s' + m + '_s' + n).fsName;
                    var myExpPathFull = new File(myArticlePathFull + '_s' + m + '_s' + n).fsName;
                    var myFileFormat = (mySlideShowData['format'] == 'PNG') ? '.' + mySlideShowData['format'].toLowerCase() : '.jpg';
                    if (kEXPORT_FORMAT == 'PDF') {
                        myFileFormat = '.pdf';
                    }
                    var myStateBaseName = myArticlePathRel + '_s' + m + '_s' + n;
                    mySlideShow.activeStateIndex = n;
                    var mySlideData = {
                        name: myState.name,
                        file: myStateBaseName + myFileFormat.toLowerCase(),
                    }
                    if (kEXPORT_FORMAT == 'JPG') {
                        if (this.tmLayoutDimensions() == '1024x768' && (!TMExporter.exportDeviceType || !TMExporter.exportDeviceType.tmStartsWith('phone_'))) {
                            TMExporter.exportTransparentObject(this, mySlideShow, myExpPath + '@2x' + myFileFormat, offsetX, offsetY, 0, 0, gBaseResolution * 2, 1.0, 1.0, true, kEXPORT_FORMAT);
                        }
                        if (this.tmLayoutDimensions() == '1280x800' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('android'))) {
                            TMExporter.exportTransparentObject(this, mySlideShow, myExpPath + myFileFormat, offsetX, offsetY, 0, 0, gBaseResolution, 1.0, 1.0, true, kEXPORT_FORMAT);
                        }
                        if (this.tmLayoutDimensions() == '1024x600' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('android'))) {
                            TMExporter.exportTransparentObject(this, mySlideShow, myExpPath + myFileFormat, offsetX, offsetY, 0, 0, gBaseResolution, 1.0, 1.0, true, kEXPORT_FORMAT);
                        }
                        if (this.tmLayoutDimensions() == '568x320' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('phone_'))) {
                            TMExporter.exportTransparentObject(this, mySlideShow, myExpPath.replace('568x320', '736x414') + '@3x' + myFileFormat, offsetX, offsetY, 0, 0, gBaseResolution * (2208 / 568), 1.0, 1.0, false, kEXPORT_FORMAT);
                        }
                        if (mySlideShowData['allowFullScreen'] == "yes" || mySlideShowData['allowFullScreen'] == true) {
                            var myFileFormat = '.jpg';
                            TMProgressBar.updateSubLabel("Slide show " + mySlideShowData['analyticsName'] + " (full slide " + (n + 1) + ' of ' + mySlideShowStatesCount + ")");
                            if (this.tmLayoutDimensions() == '1024x768' && (!TMExporter.exportDeviceType || !TMExporter.exportDeviceType.tmStartsWith('phone_'))) {
                                TMExporter.exportTransparentObject(this, mySlideShow, myExpPath + '_full@2x' + myFileFormat, offsetX, offsetY, 0, 0, gBaseResolution * 2, scale, scale, true, kEXPORT_FORMAT);
                            }
                            if (this.tmLayoutDimensions() == '1280x800' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('android'))) {
                                TMExporter.exportTransparentObject(this, mySlideShow, myExpPath + '_full' + myFileFormat, offsetX, offsetY, 0, 0, gBaseResolution, scale, scale, true, kEXPORT_FORMAT);
                            }
                            if (this.tmLayoutDimensions() == '1024x600' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('android'))) {
                                TMExporter.exportTransparentObject(this, mySlideShow, myExpPath + '_full' + myFileFormat, offsetX, offsetY, 0, 0, gBaseResolution, scale, scale, true, kEXPORT_FORMAT);
                            }
                            if (this.tmLayoutDimensions() == '568x320' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('phone_'))) {
                                TMExporter.exportTransparentObject(this, mySlideShow, myExpPath.replace('568x320', '736x414') + '_full@3x' + myFileFormat, offsetX, offsetY, 0, 0, gBaseResolution * (2208 / 568), scale, scale, false, kEXPORT_FORMAT);
                            }
                            mySlideData['full'] = myStateBaseName + '_full' + myFileFormat;
                            mySlideData['scale'] = scale;
                        }
                    }
                    if (kEXPORT_FORMAT == 'PDF') {
                        var myFileFormat = '.pdf';
                        if (mySlideShowData['allowFullScreen'] == "no" || mySlideShowData['allowFullScreen'] == false) {
                            scale = 1.0;
                        }
                        TMProgressBar.updateSubLabel("Slide show " + mySlideShowData['analyticsName'] + " (slide " + (n + 1) + ' of ' + mySlideShowStatesCount + ")");
                        if (this.tmLayoutDimensions() == '1024x768' && (!TMExporter.exportDeviceType || !TMExporter.exportDeviceType.tmStartsWith('phone_'))) {
                            TMExporter.exportTransparentObject(this, mySlideShow, myExpPath + '@2x' + myFileFormat, offsetX, offsetY, 0, 0, gBaseResolution * 2, scale, scale, true, kEXPORT_FORMAT);
                        }
                        if (this.tmLayoutDimensions() == '1280x800' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('android'))) {
                            TMExporter.exportTransparentObject(this, mySlideShow, myExpPath + myFileFormat, offsetX, offsetY, 0, 0, gBaseResolution, scale, scale, true, kEXPORT_FORMAT);
                        }
                        if (this.tmLayoutDimensions() == '1024x600' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('android'))) {
                            TMExporter.exportTransparentObject(this, mySlideShow, myExpPath + myFileFormat, offsetX, offsetY, 0, 0, gBaseResolution, scale, scale, true, kEXPORT_FORMAT);
                        }
                        if (this.tmLayoutDimensions() == '568x320' && (!TMExporter.exportDeviceType || TMExporter.exportDeviceType.tmStartsWith('phone_'))) {
                            TMExporter.exportTransparentObject(this, mySlideShow, myExpPath.replace('568x320', '736x414') + '@3x' + myFileFormat, offsetX, offsetY, 0, 0, gBaseResolution * (2208 / 568), scale, scale, false, kEXPORT_FORMAT);
                        }
                        mySlideData['file'] = myStateBaseName + myFileFormat;
                        if (mySlideShowData['allowFullScreen'] == "yes" || mySlideShowData['allowFullScreen'] == true) {
                            mySlideData['full'] = myStateBaseName + myFileFormat;
                            mySlideData['scale'] = scale;
                        }
                    }
                    mySlideData['weblinks'] = this.tmWebLinks(myState);
                    mySlideData['pagelinks'] = this.tmPageLinks(myState);
                    mySlideData['actions'] = this.tmActions(myTmpPublication, myArticlePathRel, myArticlePathFull, myState);
                    mySlideData['movies'] = this.tmMovies(myState, myTmpPublication);
                    mySlideData['sounds'] = this.tmSounds(myState, myTmpPublication);
                    mySlideData['weboverlays'] = this.tmWebOverlays(myState);
                    mySlideData['webviewers'] = this.tmWebViewers(myTmpPublication, myArticlePathRel, myState);
                    mySlideData['weblinks'] = TMGeometry.offsetObjectsWithXandY(mySlideData['weblinks'], offsetX, offsetY);
                    mySlideData['pagelinks'] = TMGeometry.offsetObjectsWithXandY(mySlideData['pagelinks'], offsetX, offsetY);
                    mySlideData['actions'] = TMGeometry.offsetObjectsWithXandY(mySlideData['actions'], offsetX, offsetY);
                    mySlideData['movies'] = TMGeometry.offsetObjectsWithXandY(mySlideData['movies'], offsetX, offsetY);
                    mySlideData['sounds'] = TMGeometry.offsetObjectsWithXandY(mySlideData['sounds'], offsetX, offsetY);
                    mySlideData['weboverlays'] = TMGeometry.offsetObjectsWithXandY(mySlideData['weboverlays'], offsetX, offsetY);
                    mySlideData['webviewers'] = TMGeometry.offsetObjectsWithXandY(mySlideData['webviewers'], offsetX, offsetY);
                    mySlideShowData['stopAtState'] = mySlideData['name'];
                    mySlideShowData['slides'].push(mySlideData);
                } catch (myException) {
                    TMLogger.error('Failed to export slide: ' + myException + ' (line ' + myException.line + ')');
                }
            }
            mySlideShowData['transitionStyle'] = mySlideShowData['transitionStyle'].replace('Cross Fade', 'crossfade').toLowerCase();
            delete mySlideShowData['format'];
            myMultistates.push(mySlideShowData);
            mySlideShow.activeStateIndex = 0;
        }
        return myMultistates;
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmMultiStateObjects", myException + ' (tm_page:2234)');
        return [];
    }
};
Page.prototype.tmSlideShows = function (myTmpPublication, myArticlePathRel, myArticlePathFull, myOptions, myOrientation, myEmptyPngName) {
    try {
        var multistates1 = this.tmMultiStateObjects(myTmpPublication, myArticlePathRel, myArticlePathFull, myOptions, myOrientation, myEmptyPngName);
        var multistates2 = this.tmFullScreenImages(myTmpPublication, myArticlePathRel, myArticlePathFull, myOptions, myOrientation, myEmptyPngName);
        return multistates1.concat(multistates2).reverse();
    } catch (myException) {
        TMLogger.exception("Page.prototype.tmSlideShows", myException + ' (tm_page:2250)');
        return [];
    }
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMPageItem = {
    tmChildren: {},
    tmScrollableProperties: {},
    tmFullScreenImageProperties: {},
    tmIsScrollable: {},
    tmIsFullScreenImage: {},
    tmIsSlideShow: {},
    tmParentPage: {},
    tmListParents: {},
    tmParents: {},
    tmDescription: {},
    tmIsNested: {},
    tmGetPageItemChildren: {},
    tmClearCache: function () {
        this.tmChildren = {};
        this.tmScrollableProperties = {};
        this.tmFullScreenImageProperties = {};
        this.tmIsScrollable = {};
        this.tmIsFullScreenImage = {};
        this.tmIsSlideShow = {};
        this.tmParentPage = {};
        this.tmListParents = {};
        this.tmParents = {};
        this.tmDescription = {};
        this.tmIsNested = {};
        this.tmGetPageItemChildren = {};
    },
    properties: function (myPageItem, label) {
        try {
            var myLabel = myPageItem.extractLabel(label);
            if (myLabel != '') {
                return TMJSON.parse(myLabel);
            }
            return {};
        } catch (myException) {
            return {};
        }
    },
    scrollableProperties: function (myPageItem) {
        try {
            if (this.tmScrollableProperties[myPageItem.id] == undefined) {
                this.tmScrollableProperties[myPageItem.id] = this.properties(myPageItem, "com.rovingbird.epublisher.sc");
            }
            return this.tmScrollableProperties[myPageItem.id];
        } catch (myException) {
            TMLogger.exception("TMPageItem.scrollableProperties", myException + ' (tm_pageitem:56)');
            return {};
        }
    },
    fullScreenImageProperties: function (myPageItem) {
        try {
            if (this.tmFullScreenImageProperties[myPageItem.id] == undefined) {
                this.tmFullScreenImageProperties[myPageItem.id] = this.properties(myPageItem, "com.rovingbird.epublisher.image");
            }
            return this.tmFullScreenImageProperties[myPageItem.id];
        } catch (myException) {
            TMLogger.exception("TMPageItem.fullScreenImageProperties", myException + ' (tm_pageitem:68)');
            return {};
        }
    },
    isInteractiveElement: function (myPageItem) {
        if (TMPageItem.isScrollable(myPageItem)) {
            return true;
        }
        if (TMPageItem.isSlideShow(myPageItem)) {
            return true;
        }
        var labelsToTest = [
            'com.rovingbird.epublisher.wv',
            'com.rovingbird.epublisher.wo',
            'com.rovingbird.epublisher.sc',
            'com.rovingbird.epublisher.movie',
            'com.rovingbird.epublisher.image',
            'com.rovingbird.epublisher.imagesequence',
            'com.rovingbird.epublisher.widget',
            'com.rovingbird.epublisher.panorama',
            'com.rovingbird.epublisher.mso',
        ];
        for (var i = 0; i < labelsToTest.length; i++) {
            var label = labelsToTest[i];
            var myLabel = myPageItem.extractLabel(label);
            if (myLabel != '') {
                return true;
            }
        }
        return false;
    },
    analyticsName: function (myPageItem) {
        var labelsToTest = [
            'com.rovingbird.epublisher.wv',
            'com.rovingbird.epublisher.wo',
            'com.rovingbird.epublisher.sc',
            'com.rovingbird.epublisher.movie',
            'com.rovingbird.epublisher.image',
            'com.rovingbird.epublisher.imagesequence',
            'com.rovingbird.epublisher.widget',
            'com.rovingbird.epublisher.panorama',
            'com.rovingbird.epublisher.mso',
        ];
        for (var i = 0; i < labelsToTest.length; i++) {
            var label = labelsToTest[i];
            var myLabel = myPageItem.extractLabel(label);
            if (myLabel != '') {
                var parsedLabel = TMJSON.parse(myLabel);
                for (var propertyName in parsedLabel) {
                    if (propertyName.toLowerCase().tmEndsWith('analyticsname')) {
                        return parsedLabel[propertyName];
                    }
                }
            }
        }
        return myPageItem.id;
    },
    isScrollable: function (myPageItem) {
        try {
            if (!myPageItem.hasOwnProperty('id') || !myPageItem.hasOwnProperty('allPageItems')) {
                return false;
            }
            if (myPageItem.allPageItems.length == 0) {
                return false;
            }
            if (this.tmIsScrollable[myPageItem.id] == undefined) {
                var myProps = this.scrollableProperties(myPageItem);
                if (myProps.scAllowScrolling && myPageItem.allPageItems.length > 0) {
                    this.tmIsScrollable[myPageItem.id] = true;
                } else {
                    this.tmIsScrollable[myPageItem.id] = false;
                }
            }
            return this.tmIsScrollable[myPageItem.id];
        } catch (myException) {
            TMLogger.exception("TMPageItem.isScrollable", myException + ' (tm_pageitem:146)');
            return false;
        }
    },
    isFullScreenImage: function (myPageItem) {
        try {
            if (!myPageItem.hasOwnProperty('id')) {
                return false;
            }
            if (this.tmIsFullScreenImage[myPageItem.id] == undefined) {
                var myProps = this.fullScreenImageProperties(myPageItem);
                if (myProps.imageAllowFullScreen) {
                    this.tmIsFullScreenImage[myPageItem.id] = true;
                } else {
                    this.tmIsFullScreenImage[myPageItem.id] = false;
                }
            }
            return this.tmIsFullScreenImage[myPageItem.id];
        } catch (myException) {
            TMLogger.exception("TMPageItem.isFullScreenImage", myException + ' (tm_pageitem:166)');
            return false;
        }
    },
    isSlideShow: function (myPageItem) {
        try {
            if (!myPageItem.hasOwnProperty('id')) {
                return false;
            }
            if (this.tmIsSlideShow[myPageItem.id] == undefined) {
                this.tmIsSlideShow[myPageItem.id] = myPageItem instanceof MultiStateObject;
            }
            return this.tmIsSlideShow[myPageItem.id];
        } catch (myException) {
            TMLogger.exception("TMPageItem.isSlideShow", myException + ' (tm_pageitem:181)');
            return false;
        }
    },
    parentPage: function (myPageItem) {
        try {
            if (!myPageItem || !myPageItem.isValid) {
                return undefined;
            }
            if (!myPageItem.hasOwnProperty('id')) {
                return undefined;
            }
            var myParentPage = myPageItem.parentPage;
            if (!myParentPage) {
                var myItem = myPageItem;
                while (myItem.hasOwnProperty('parent')) {
                    try {
                        myItem = myItem.parent;
                    } catch (myException) {
                        break;
                    }
                    if (myItem.hasOwnProperty('parentPage') && myItem.parentPage) {
                        myParentPage = myItem.parentPage;
                        break;
                    }
                    if (myItem instanceof Document) {
                        break;
                    }
                }
            }
            this.tmParentPage[myPageItem.id] = myParentPage;
            return this.tmParentPage[myPageItem.id];
        } catch (myException) {
            TMLogger.exception("TMPageItem.parentPage", myException + ' (tm_pageitem:226)');
            return undefined;
        }
    },
    listParents: function (myPageItem) {
        try {
            if (this.tmListParents[myPageItem.id] == undefined) {
                var myParents = [this.description(myPageItem)];
                var myItem = myPageItem;
                while (myItem.parent != undefined) {
                    myItem = myItem.parent;
                    if (myItem instanceof Document) {
                        break;
                    }
                    myParents.push(this.description(myItem));
                }
                myParents = myParents.reverse();
                var myParentDesc = '';
                var j = 0;
                for (var i in myParents) {
                    myParentDesc += '-'.tmRepeat(j) + ' ' + myParents[i] + '\n';
                    j++;
                }
                this.tmListParents[myPageItem.id] = myParentDesc.tmTrim();
            }
            return this.tmListParents[myPageItem.id];
        } catch (myException) {
            TMLogger.exception("TMPageItem.listParents", myException + ' (tm_pageitem:268)');
            return this.description(myPageItem);
        }
    },
    parents: function (myPageItem) {
        try {
            if (this.tmParents[myPageItem.id] == undefined) {
                var myParents = [];
                var myItem = myPageItem;
                while (myItem.parent != undefined) {
                    myItem = myItem.parent;
                    if (myItem instanceof Document) {
                        break;
                    }
                    myParents.push(myItem);
                }
                this.tmParents[myPageItem.id] = myParents;
            }
            return this.tmParents[myPageItem.id];
        } catch (myException) {
            TMLogger.exception("TMPageItem.parents", myException + ' (tm_pageitem:299)');
            return this.description(myPageItem);
        }
    },
    parentIDs: function (myPageItem) {
        var myIDs = [];
        var myParents = this.parents(myPageItem);
        for (var i = 0; i < myParents.length; i++) {
            var myParent = myParents[i];
            myIDs.push(myParent.id);
        }
        return myIDs;
    },
    description: function (myPageItem) {
        try {
            if (myPageItem == undefined) {
                return '[undefined]';
            }
            if (this.tmDescription[myPageItem.id] == undefined) {
                this.tmDescription[myPageItem.id] = this.getPageItemID(myPageItem) + ' ' + myPageItem;
            }
            return this.tmDescription[myPageItem.id];
        } catch (myException) {
            TMLogger.exception("PageItem.prototype.tmDescription", myException + ' (tm_pageitem:324)');
            return "";
        }
    },
    isNested: function (myPageItem) {
        try {
            var myItem = myPageItem;
            if (!myPageItem) {
                return false;
            }
            if (this.tmIsNested[myPageItem.id] != undefined) {
                return this.tmIsNested[myPageItem.id];
            }
            this.tmIsNested[myPageItem.id] = false
            while (true) {
                if (!myItem || !myItem.isValid) {
                    break;
                }
                myParent = myItem.parent;
                if (myItem.constructor.name == 'Character') {
                    myParent = myItem.parentTextFrames[0];
                }
                if (myParent.constructor.name == 'Spread' || myParent.constructor.name == 'MasterSpread' || myParent.constructor.name == 'Document' || myParent.constructor.name == 'Application') {
                    break;
                }
                if (this.isScrollable(myParent) || this.isSlideShow(myParent) || this.isFullScreenImage(myParent)) {
                    this.tmIsNested[myPageItem.id] = true;
                    break;
                }
                myItem = myParent;
            }
            return this.tmIsNested[myPageItem.id];
        } catch (myException) {
            TMLogger.exception("TMPageItem.isNested", myException + ' (tm_pageitem:364)');
            return false;
        }
    },
    isChildOf: function (myPageItem, myParentItem) {
        try {
            try {
                var myID = myParentItem.id;
                var myParentIDs = this.parentIDs(myPageItem);
                if (TMUtilities.itemInArray(myParentIDs, myID)) {
                    return true;
                }
            } catch (myException) { }
            try {
                if (myPageItem.id == myParentItem.id) {
                    return true;
                }
            } catch (myException) { }
            var myParentChildren = this.getPageItemChildren(myParentItem);
            for (var i = 0; i < myParentChildren.length; i++) {
                var myParentChild = myParentChildren[i];
                if (myParentChild.id == myPageItem.id) {
                    return true;
                }
            }
            return false;
        } catch (myException) {
            TMLogger.exception("TMPageItem.isChildOf", myException + ' (tm_pageitem:403)');
            return false;
        }
    },
    getPageItemID: function (myPageItem) {
        try {
            if (myPageItem == undefined) {
                return false;
            }
            var myItem = myPageItem;
            while (true) {
                if (myItem.hasOwnProperty('id')) {
                    return myItem.id;
                }
                myParent = myItem.parent;
                if (myParent == undefined || myParent.constructor.name == 'Spread' || myParent.constructor.name == 'MasterSpread' || myParent.constructor.name == 'Document' || myParent.constructor.name == 'Application') {
                    break;
                }
                myItem = myParent;
            }
            return false;
        } catch (myException) {
            TMLogger.exception("TMPageItem.getPageItemID", myException + ' (tm_pageitem:426)');
            return false;
        }
    },
    getPageItemChildren: function (myPageItem) {
        try {
            if (myPageItem == undefined) {
                return [];
            }
            TMPageItem.tmGetPageItemChildren[myPageItem.id] = [];
            var myItems = [];
            if (myPageItem.hasOwnProperty('allPageItems')) {
                myItems = myPageItem.allPageItems.slice(0);
            } else if (myPageItem.hasOwnProperty('pageItems')) {
                myItems = myPageItem.pageItems;
            }
            if (myItems.length == 0) {
                return [];
            }
            if (myItems.constructor.name == 'PageItems') {
                myItems = TMUtilities.collectionToArray(myItems);
            }
            if (myItems instanceof PageItem) {
                myItems = [myItems];
            }
            var myCount = myItems.length;
            for (var i = 0; i < myCount; i++) {
                var myChildren = this.getPageItemChildren(myItems[i]);
                for (var j in myChildren) {
                    myItems.push(myChildren[j]);
                }
            };
            TMPageItem.tmGetPageItemChildren[myPageItem.id] = this.removeDuplicatePageItems(myItems);
            return TMPageItem.tmGetPageItemChildren[myPageItem.id];
        } catch (myException) {
            TMLogger.exception("TMPageItem.getPageItemChildren", myException + ' (tm_pageitem:482)');
            return [];
        }
    },
    removeDuplicatePageItems: function (myItems) {
        try {
            var myFilteredItems = [];
            var myObjectIDs = {};
            for (var i in myItems) {
                var myItem = myItems[i];
                if (myObjectIDs[myItem.id] != undefined) {
                    continue;
                }
                myFilteredItems.push(myItem);
                myObjectIDs[myItem.id] = 1;
            }
            return myFilteredItems;
        } catch (myException) {
            TMLogger.exception("TMPageItem.removeDuplicatePageItems", myException + ' (tm_pageitem:528)');
            return myItems;
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var gLastFolder = Folder.desktop;
var gBaseResolution = 72;
var TMPluginCore = {
    engine: undefined,
    appVersion: function () {
        return parseInt(app.version.split('.').slice(0, 3).join(''));
    },
    appDisplayVersion: function () {
        var version = parseInt(this.appVersion().toString().substr(0, 3));
        if (version == 110) {
            return "Adobe InDesign CC 2015";
        }
        if (version == 102) {
            return "Adobe InDesign CC 2014.2";
        }
        if (version == 101) {
            return "Adobe InDesign CC 2014.1";
        }
        if (version == 100) {
            return "Adobe InDesign CC 2014";
        }
        if (version == 90) {
            return "Adobe InDesign CC";
        }
        if (version == 80) {
            return "Adobe InDesign CS6";
        }
        if (version == 75) {
            return "Adobe InDesign CS5.5";
        }
        if (version == 70) {
            return "Adobe InDesign CS5";
        }
        return "Adobe InDesign " + this.appVersion();
    },
    main: function (engine) {
        try {
            TMPluginCore.engine = engine;
            var indesignPath = Folder.startup;
            if (File.fs == "Macintosh") {
                indesignPath = indesignPath.parent.parent;
            }
            TMLogger.separator("");
            TMLogger.info('Twixl Publisher ' + TMVersion.version + ' (' + TMVersion.build + ') for ' + File.fs);
            TMLogger.info("");
            TMLogger.info("Copyright (c) Copyright Twixl media. All rights reserved.");
            TMLogger.separator("");
            TMLogger.info("   InDesign Path: " + indesignPath.fsName);
            TMLogger.info("InDesign Version: " + this.appDisplayVersion());
            TMLogger.info("      OS Version: " + $.os);
            TMLogger.info("   Target Engine: " + $.engineName);
            TMLogger.separator("");
            TMPluginCore.startHelperIfNeeded()
            TMCache.cleanup();
            if (engine == 'html') {
                TMEventHandlers.init();
                TMPreferences.dispatchPreferencesUpdated();
            }
            TMLogger.info("Plugin Startup Complete");
        } catch (myException) {
            TMLogger.exception('TMPluginCore.main', myException + ' (tm_plugincore:78)');
        }
    },
    startHelperIfNeeded: function () {
        if (!TMHelper.isRunning()) {
            TMHelper.startHelper();
        }
    },
}
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMPreferences = {
    dispatchPreferencesUpdated: function () {
        TMLogger.info('Updated the preferences');
        TMUtilities.dispatchEvent(
            'com.twixlmedia.publisher.indesign.events.preferencesUpdated',
            TMPreferences.readAll()
        );
    },
    isRunningLegacyMode: function () {
        return TMPreferences.readObject('enableLegacyOptions', false);
    },
    isDebug: function () {
        return TMPreferences.readObject('enableDebugging', false);
    },
    saveObject: function (myKey, myValue) {
        try {
            var preferences = this.readAll();
            preferences[myKey] = myValue;
            this.saveAll(preferences);
        } catch (myException) {
            TMLogger.exception("TMPreferences.saveObject", myException + ' (tm_preferences:30)');
        }
    },
    readObject: function (myKey, myDefaultValue) {
        try {
            var preferences = this.readAll();
            if (!preferences) {
                return myDefaultValue;
            }
            if (!preferences.hasOwnProperty(myKey)) {
                return myDefaultValue;
            }
            if (preferences[myKey] == undefined) {
                return myDefaultValue;
            }
            return preferences[myKey];
        } catch (myException) {
            TMLogger.exception("TMPreferences.readObject", myException + ' (tm_preferences:48)');
            return myDefaultValue;
        }
    },
    readInt: function (myKey, myDefaultValue) {
        try {
            var myValue = this.readObject(myKey, myDefaultValue);
            var myValueAsInt = parseInt(myValue);
            return isNaN(myValueAsInt) ? myDefaultValue : myValueAsInt;
        } catch (myException) {
            TMLogger.exception("TMPreferences.readInt", myException + ' (tm_preferences:59)');
            return myDefaultValue;
        }
    },
    saveArray: function (myKey, myValue) {
        try {
            this.saveObject(myKey, myValue);
        } catch (myException) {
            TMLogger.exception("TMPreferences.saveArray", myException + ' (tm_preferences:68)');
        }
    },
    readArray: function (myKey, myDefaultValue) {
        try {
            return this.readObject(myKey, myDefaultValue);
        } catch (myException) {
            TMLogger.exception("TMPreferences.readArray", myException + ' (tm_preferences:76)');
            return myDefaultValue;
        }
    },
    remove: function (myKey) {
        try {
            var preferences = this.readAll();
            delete preferences[myKey];
            this.saveAll(preferences);
        } catch (myException) {
            TMLogger.exception("TMPreferences.remove", myException + ' (tm_preferences:87)');
        }
    },
    prefsFile: function () {
        var prefsPath = '';
        if (File.fs == 'Windows') {
            prefsPath = Folder.myDocuments.fsName + '/Twixl Publisher/Preferences';
        } else {
            prefsPath = '~/Library/Application Support/Twixl Publisher Plugin';
        }
        return prefsPath + '/Twixl Publisher Preferences.json';
    },
    readAll: function () {
        try {
            var prefsFile = new File(this.prefsFile());
            if (!prefsFile.exists) {
                return {};
            }
            var prefsData = TMFiles.readUTF8DataFromFile(this.prefsFile());
            try {
                return TMJSON.parse(prefsData);
            } catch (myException) {
                return {};
            }
        } catch (myException) {
            TMLogger.exception("TMPreferences.readAll", myException + ' (tm_preferences:117)');
            return {};
        }
    },
    saveAll: function (object) {
        try {
            var prefsFile = new File(this.prefsFile());
            if (!prefsFile.parent.parent.exists) {
                prefsFile.parent.parent.create();
            }
            if (!prefsFile.parent.exists) {
                prefsFile.parent.create();
            }
            var prefsData = TMJSON.stringify(object);
            TMFiles.writeUTF8DataToFile(this.prefsFile(), prefsData);
        } catch (myException) {
            TMLogger.exception("TMPreferences.saveAll", myException + ' (tm_preferences:138)');
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMPreflighter = {
    warnings: [],
    errors: [],
    articleNames: [],
    reportFullName: undefined,
    preflightName: undefined,
    reset: function () {
        this.warnings = {};
        this.errors = {};
        this.articleNames = [];
        this.preflightName = undefined;
    },
    addWarning: function (category, message) {
        try {
            if (!this.warnings.hasOwnProperty(category)) {
                this.warnings[category] = [];
            }
            var msgLowerCase = message.toLowerCase();
            if (!TMUtilities.itemInArray(this.warnings[category], msgLowerCase)) {
                this.warnings[category].push(message);
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.addWarning', myException);
        }
    },
    addError: function (category, message) {
        try {
            if (!this.errors.hasOwnProperty(category)) {
                this.errors[category] = [];
            }
            var msgLowerCase = message.toLowerCase();
            if (!TMUtilities.itemInArray(this.errors[category], msgLowerCase)) {
                this.errors[category].push(message);
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.addError', myException);
        }
    },
    warningCount: function () {
        try {
            var warningCount = 0;
            for (var myCategory in this.warnings) {
                warningCount += this.warnings[myCategory].length;
            }
            return warningCount;
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.warningCount', myException);
        }
    },
    errorCount: function () {
        try {
            var errorCount = 0;
            for (var myCategory in this.errors) {
                errorCount += this.errors[myCategory].length;
            }
            return errorCount;
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.errorCount', myException);
        }
    },
    preflight: function (args) {
        try {
            var myArticleName = args[0];
            var myDeviceType = args[1];
            var myAction = args[2];
            TMTimer.start('preflight', 'Starting preflight of publication: ' + app.activeBook.name);
            this.reset();
            TMBook.resetCache();
            TMExporter.reset();
            TMCache.reset();
            this.preflightName = app.activeBook.name;
            var myProgressSteps = app.activeBook.tmFileCount();
            TMProgressBar.create('Preflight publication', 'Preflighting...', myProgressSteps);
            TMLogger.info('Book Properties: ' + TMJSON.stringify(app.activeBook.tmProperties()));
            var myErrors = app.activeBook.tmSaveModifiedDocuments(myArticleName);
            if (myErrors.length > 0) {
                this.errors = this.errors.concat(myErrors);
            }
            if (this.errors.length > 0) {
                this.printErrors();
                return false;
            }
            this.articleNames = app.activeBook.tmArticleNames();
            var publicationLayouts = {};
            var myPageNumbers = {};
            var myOrientations = {};
            var myBookContentsCount = app.activeBook.tmFileCount();
            for (var i = 0; i < myBookContentsCount; i++) {
                var myFile = app.activeBook.bookContents.item(i);
                if (myArticleName != undefined && myArticleName != '' && myFile.tmArticleName() != myArticleName) {
                    TMProgressBar.updateProgressBySteps(1);
                    continue;
                }
                try {
                    TMLogger.info("Opening: " + TMUtilities.decodeURI(myFile.fullName));
                    myDocument = app.open(myFile.fullName, false, OpenOptions.OPEN_COPY);
                } catch (myException) {
                    TMLogger.error(myException);
                    this.addError(TMUtilities.decodeURI(myFile.name), 'File "' + myFile.fullName + '" cannot be opened: ' + myException);
                    TMProgressBar.updateProgressBySteps(1);
                    continue;
                }
                if (myDocument) {
                    var documentProperties = myDocument.tmProperties();
                    documentProperties['originalFileName'] = myFile.fullName;
                    documentProperties['originalBaseName'] = TMUtilities.decodeURI(myFile.name);
                    myDocument.label = documentProperties.toSource();
                    var myPageLabel = (myDocument.pages.count() == 1) ? 'page' : 'pages';
                    TMProgressBar.update('Preflighting: ' + TMUtilities.decodeURI(myFile.name), myDocument.pages.count() + ' ' + myPageLabel);
                    TMLogger.info('Preflighting: ' + myFile.fullName.fsName + ' (alternate layouts: ' + myDocument.tmUsesAlternateLayouts() + ')');
                    myDocument.tmPrepareForExport(false);
                    TMPreflighter.reportFullName = TMUtilities.decodeURI(myFile.name);
                    gBaseResolution = myDocument.tmDocumentBaseResolution();
                    this.checkFacingPages(myDocument);
                    this.checkMissingLinks(myDocument);
                    this.checkMissingFonts(myDocument);
                    this.checkImageResolutions(myDocument);
                    this.checkBackgroundMusic(myDocument);
                    if (myDocument.tmUsesAlternateLayouts()) {
                        publicationLayouts[myFile.fullName.fsName] = {
                            'reportName': this.reportFullName,
                            'layouts': TMAlternateLayouts.documentLayouts(myDocument)
                        };
                        this.checkAlternateLayoutNames(myDocument);
                        this.checkAlternateLayoutPageCounts(myDocument);
                    } else {
                        var myDocName = TMFiles.getReportName(myFile.fullName.fsName);
                        var myPageCount = myDocument.pages.length;
                        var myOrientation = myDocument.tmOrientation();
                        if (myOrientations[myDocName] == undefined) {
                            myOrientations[myDocName] = [];
                        }
                        myOrientations[myDocName].push(myOrientation);
                        if (myPageNumbers[myDocName] == undefined) {
                            myPageNumbers[myDocName] = myPageCount;
                        } else {
                            var myOtherPageCount = myPageNumbers[myDocName];
                            if (myPageCount != myOtherPageCount) {
                                if (myOrientation == 'portrait') {
                                    var portraitPageCount = myPageCount;
                                    var landscapePageCount = myOtherPageCount;
                                } else {
                                    var portraitPageCount = myOtherPageCount;
                                    var landscapePageCount = myPageCount;
                                }
                                this.errors.push('Article "' + myDocName + '" has a different page count for portrait (' + portraitPageCount + ') and landscape (' + landscapePageCount + ').');
                                this.addError(myDocName, 'Article has a different page count for portrait (' + portraitPageCount + ') and landscape (' + landscapePageCount + ').')
                            }
                        }
                    }
                    var myPages = TMUtilities.collectionToArray(myDocument.pages);
                    var myPagesCount = myPages.length;
                    for (var j = 0; j < myPagesCount; j++) {
                        var myPage = myPages[j];
                        if (!TMUtilities.itemInArray(TMExporter.supportedLayouts, myPage.tmLayoutName())) {
                            continue;
                        }
                        TMLogger.debug('    Preflighting page ' + (j + 1) + ' of ' + myPagesCount);
                        myPage.tmPrepareForExport(false);
                        this.checkPageSizes(myPage);
                        this.checkScrollableContent(myPage);
                        this.checkImageSequences(myPage);
                        this.checkPanoramas(myPage);
                        this.checkRotationAngles(myPage);
                    }
                    TMProgressBar.updateProgressBySteps(1);
                    try {
                        myDocument.close(SaveOptions.NO);
                    } catch (myException) {
                        TMLogger.exception("TMPreflighter.preflight", myException + ' (tm_preflighter:216)');
                    }
                }
            }
            if (app.activeBook.tmUsesAlternateLayouts()) {
                var exportPublicationLayouts = this.checkAlternateLayoutsInPublication(publicationLayouts);
                TMExporter.exportPublicationLayouts = exportPublicationLayouts;
            } else {
                TMExporter.exportPublicationLayouts = ['ipad'];
                var myRequiredOrientations = app.activeBook.tmRequiredOrientations();
                for (var myArticle in myOrientations) {
                    for (var myOrientation in myOrientations[myArticle]) {
                        var myOrientationName = myOrientations[myArticle][myOrientation];
                        if (!TMUtilities.itemInArray(myRequiredOrientations, myOrientationName)) {
                            this.addError(myArticle, 'Article should not have the ' + myOrientationName + ' orientation.');
                            return;
                        }
                    }
                    for (var myOrientation in myRequiredOrientations) {
                        var myOrientationName = myRequiredOrientations[myOrientation];
                        if (!TMUtilities.itemInArray(myOrientations[myArticle], myOrientationName)) {
                            this.addError(myArticle, 'Article is missing the ' + myOrientationName + ' orientation.');
                            return;
                        }
                    }
                }
            }
            this.printErrors(myAction);
            if (this.errorCount() > 0 || this.warningCount() > 0) {
                return false;
            } else {
                return true;
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.preflight', myException);
        }
    },
    preflightArticle: function (args) {
        try {
            var myArticleFile = args[0];
            var myFile = new File(myArticleFile);
            var myAction = args[2];
            TMTimer.start('preflight', 'Starting preflight of article: ' + myArticleFile);
            this.reset();
            TMExporter.reset();
            TMCache.reset();
            this.preflightName = TMUtilities.decodeURI(myFile.name);
            this.reportFullName = TMUtilities.decodeURI(myFile.name);
            try {
                TMLogger.info("Opening: " + TMUtilities.decodeURI(myFile));
                if (TMApplication.isServer()) {
                    myDocument = app.open(myFile, OpenOptions.OPEN_COPY);
                } else {
                    myDocument = app.open(myFile, false, OpenOptions.OPEN_COPY);
                }
            } catch (myException) {
                TMLogger.exception("TMPreflighter.preflightArticle", myException + ' (tm_preflighter:291)');
                throw myException;
            }
            if (myDocument) {
                var myPages = TMUtilities.collectionToArray(myDocument.pages);
                var myPagesCount = myPages.length;
                TMProgressBar.create('Preflight article', 'Preflighting...', 2 + myPagesCount);
                var documentProperties = myDocument.tmProperties();
                documentProperties['originalFileName'] = myFile.fullName;
                documentProperties['originalBaseName'] = TMUtilities.decodeURI(myFile.name);
                myDocument.label = documentProperties.toSource();
                myDocument.tmPrepareForExport(false);
                if (!myDocument.tmUsesAlternateLayouts() && app.activeBook == undefined) {
                    myDocument.close(SaveOptions.NO);
                    TMLogger.exception("TMPreflighter.preflightArticle", "Only supported with alternate layouts" + ' (tm_preflighter:311)');
                    TMDialogs.error('Only supported if the document supports alternate layouts.');
                    throw 'Only supported if the document supports alternate layouts.';
                }
                var myPageLabel = (myDocument.pages.count() == 1) ? 'page' : 'pages';
                TMProgressBar.update('Preflighting: ' + TMUtilities.decodeURI(myFile.name), myDocument.pages.count() + ' ' + myPageLabel);
                TMLogger.info('Preflighting: ' + myFile.fsName + ' (alternate layouts: ' + myDocument.tmUsesAlternateLayouts() + ')');
                gBaseResolution = myDocument.tmDocumentBaseResolution();
                this.checkFacingPages(myDocument);
                this.checkMissingLinks(myDocument);
                this.checkMissingFonts(myDocument);
                this.checkImageResolutions(myDocument);
                this.checkBackgroundMusic(myDocument);
                if (myDocument.tmUsesAlternateLayouts()) {
                    this.checkAlternateLayoutNames(myDocument);
                    this.checkAlternateLayoutPageCounts(myDocument);
                }
                var myPages = TMUtilities.collectionToArray(myDocument.pages);
                var myPagesCount = myPages.length;
                for (var j = 0; j < myPagesCount; j++) {
                    var myPage = myPages[j];
                    TMProgressBar.updateSubLabel('Preflighting page ' + (j + 1) + ' of ' + myPagesCount);
                    TMLogger.info('    Preflighting page ' + (j + 1) + ' of ' + myPagesCount);
                    myPage.tmPrepareForExport(false);
                    this.checkPageSizes(myPage);
                    this.checkScrollableContent(myPage);
                    this.checkImageSequences(myPage);
                    this.checkPanoramas(myPage);
                    this.checkRotationAngles(myPage);
                    TMProgressBar.updateProgressBySteps(1);
                }
                TMProgressBar.updateProgressBySteps(1);
                try {
                    myDocument.close(SaveOptions.NO);
                } catch (myException) {
                    TMLogger.exception("TMPreflighter.preflightArticle", myException + ' (tm_preflighter:357)');
                }
            }
            this.printErrors(myAction);
            if (this.errorCount() > 0 || this.warningCount() > 0) {
                return false;
            } else {
                return true;
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.preflightArticle', myException);
        }
    },
    printErrors: function (myAction) {
        TMLogger.info('Preflight action: ' + myAction);
        var preferenceKey = (myAction && myAction.tmStartsWith('preview')) ? 'preflightWarningsOnPreview' : 'preflightWarningsOnExport';
        var showWarnings = TMPreferences.readObject(preferenceKey, true);
        if (!showWarnings) {
            TMLogger.info('Clearing the preflight errors due to preference setting');
            this.warnings = [];
        }
        TMProgressBar.close();
        var warningCount = TMPreflighter.warningCount();
        var errorCount = TMPreflighter.errorCount();
        if (errorCount > 0 || warningCount > 0) {
            TMLogger.info('Result: ' + warningCount + ' warning(s), ' + errorCount + ' error(s)');
            for (var myCategory in this.errors) {
                TMLogger.info('    File: ' + myCategory);
                for (var i = 0; i < this.errors[myCategory].length; i++) {
                    TMLogger.error('        Error: ' + this.errors[myCategory][i]);
                }
            }
        }
        TMTimer.printElapsed('preflight', 'Preflight duration');
    },
    checkPageSizes: function (myPage) {
        try {
            var usesAlternateLayouts = myPage.tmUsesAlternateLayouts();
            var reportFullName = this.reportFullName;
            var layoutName = myPage.tmLayoutName();
            if (usesAlternateLayouts) {
                var mySupportedLayouts = TMExporter.supportedLayouts;
                if (!TMUtilities.itemInArray(mySupportedLayouts, layoutName)) {
                    return
                }
                var myBaseError = 'Page ' + myPage.name + ' in layout "' + layoutName + '" has incorrect page dimensions.';
            } else {
                var myBaseError = 'Page ' + myPage.name + ' has incorrect page dimensions.';
            }
            var myWidth = myPage.tmPageWidthPx();
            var myHeight = myPage.tmPageHeightPx();
            var myReqWidth = myPage.tmScreenWidthPx();
            var myReqHeight = myPage.tmScreenHeightPx();
            if (myReqWidth == undefined || myReqHeight == undefined) {
                return;
            }
            if (myPage.tmLayoutPageCount() == 1) {
                if (myWidth != myReqWidth && myHeight < myReqHeight) {
                    this.addError(reportFullName, myBaseError + ' They should be exactly ' + myReqWidth + ' x ' + myReqHeight + 'px instead of ' + myWidth + ' x ' + myHeight + 'px.');
                } else if (myWidth != myReqWidth) {
                    this.addError(reportFullName, myBaseError + ' It should be exactly ' + myReqWidth + 'px wide instead of ' + myWidth + 'px.');
                } else if (myHeight < myReqHeight) {
                    this.addError(reportFullName, myBaseError + ' It should be exactly ' + myReqHeight + 'px high instead of ' + myHeight + 'px.');
                }
            } else {
                if (usesAlternateLayouts) {
                    if (myWidth != myReqWidth || myHeight != myReqHeight) {
                        this.addError(reportFullName, myBaseError + ' They should be exactly ' + myReqWidth + ' x ' + myReqHeight + 'px instead of ' + myWidth + ' x ' + myHeight + 'px.');
                        return;
                    }
                } else {
                    if (myWidth != myReqWidth) {
                        this.addError(reportFullName, myBaseError + ' It should be exactly ' + myReqWidth + 'px wide instead of ' + myWidth + 'px.');
                        return;
                    }
                    if (myHeight < (myReqHeight - 20)) {
                        this.addError(reportFullName, myBaseError + ' It should be minimum ' + (myReqHeight - 20) + 'px high instead of ' + myHeight + 'px.');
                        return;
                    }
                }
            }
            var baseError = 'Page ' + myPage.name + ' in layout "' + layoutName + '" exceeds the maximum page height of ';
            if (layoutName.substr(0, 5).toLowerCase() == 'phone' && myHeight > 8400) {
                this.addError(reportFullName, baseError + '8400 px.');
                return;
            } else if (myHeight > 14400) {
                this.addError(reportFullName, baseError + '14400 px.');
                return;
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.checkPageSizes', myException);
        }
    },
    checkScrollableContent: function (myPage) {
        try {
            var reportFullName = this.reportFullName;
            var layoutName = myPage.tmLayoutName();
            var myItems = myPage.tmPageItems();
            var myCount = myItems.length;
            for (var i = 0; i < myCount; i++) {
                var myPageItem = myItems[i];
                if (TMPageItem.isScrollable(myPageItem)) {
                    var myProps = TMPageItem.scrollableProperties(myPageItem);
                    if (!myPageItem.hasOwnProperty('allPageItems')) {
                        continue;
                    }
                    if (myProps.scAllowScrolling && myPageItem.allPageItems.length > 0) {
                        var myScrollingBounds = myPageItem.geometricBounds;
                        var myScrollingContent = myPageItem.allPageItems[0];
                        var myScrollingContentBounds = myScrollingContent.geometricBounds;
                        var myScrollData = {};
                        myScrollData['showScrollbars'] = TMUtilities.getBoolProperty(myProps, 'scShowScrollbars', 'yes');
                        myScrollData['enablePaging'] = 'no'; //TMUtilities.getBoolProperty(myProps, 'scEnablePaging', 'no');
                        myScrollData['enableZooming'] = TMUtilities.getBoolProperty(myProps, 'scEnableZooming', 'no');
                        myScrollData['analyticsName'] = TMUtilities.getProperty(myProps, "scAnalyticsName", "" + myPageItem.id);
                        myScrollData = TMGeometry.addCorrectedBounds(myScrollData, myScrollingBounds);
                        myScrollData['contentX'] = TMGeometry.convertToPixels(myScrollingContentBounds[1]) - myScrollData['x'];
                        myScrollData['contentY'] = TMGeometry.convertToPixels(myScrollingContentBounds[0]) - myScrollData['y'];
                        myScrollData['contentWidth'] = Math.round(TMGeometry.convertToPixels(myScrollingContentBounds[3] - myScrollingContentBounds[1]));
                        myScrollData['contentHeight'] = Math.round(TMGeometry.convertToPixels(myScrollingContentBounds[2] - myScrollingContentBounds[0]));
                        var offsetX = 0;
                        var offsetY = 0;
                        if (myScrollData['enableZooming'] == 'no' || myScrollData['enableZooming'] == false) {
                            if (myScrollData['contentX'] > 0) {
                                offsetX = myScrollData['contentX'];
                                myScrollData['contentX'] = 0;
                                myScrollData['contentWidth'] += offsetX;
                            }
                            if (myScrollData['contentY'] > 0) {
                                offsetY = myScrollData['contentY'];
                                myScrollData['contentY'] = 0;
                                myScrollData['contentHeight'] += offsetY;
                            }
                        }
                        var paddingX = 0;
                        var paddingY = 0;
                        if (myScrollData['enableZooming'] == 'no' || myScrollData['enableZooming'] == false) {
                            paddingX = myScrollData['width'] - (myScrollData['contentX'] + myScrollData['contentWidth']);
                            if (paddingX > 0) {
                                myScrollData['contentWidth'] += paddingX;
                            }
                            paddingY = myScrollData['height'] - (myScrollData['contentY'] + myScrollData['contentHeight']);
                            if (paddingY > 0) {
                                myScrollData['contentHeight'] += paddingY;
                            }
                        }
                        var baseError = 'Scrollable item "' + myScrollData['analyticsName'] + '" on page ' + myPage.name + ' in layout "' + layoutName + '" exceeds the maximum ';
                        if (layoutName.substr(0, 5).toLowerCase() == 'phone' && myScrollData['contentWidth'] > 8400) {
                            this.addError(reportFullName, baseError + 'width of 8400 px.');
                            continue;
                        } else if (layoutName.substr('0, 4').toLowerCase() == 'ipad' && myScrollData['contentWidth'] > 15000) {
                            this.addError(reportFullName, baseError + 'width of 15000 px.');
                            continue;
                        } else if (myScrollData['contentWidth'] > 32000) {
                            this.addError(reportFullName, baseError + 'width of 32000 px.');
                            continue;
                        }
                        if (layoutName.substr(0, 5).toLowerCase() == 'phone' && myScrollData['contentHeight'] > 8400) {
                            this.addError(reportFullName, baseError + 'height of 8400 px.');
                            continue;
                        } else if (layoutName.substr('0, 4').toLowerCase() == 'ipad' && myScrollData['contentHeight'] > 15000) {
                            this.addError(reportFullName, baseError + 'height of 15000 px.');
                            continue;
                        } else if (myScrollData['contentHeight'] > 32000) {
                            this.addError(reportFullName, baseError + 'height of 32000 px.');
                            continue;
                        }
                    }
                }
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.checkScrollableContent', myException);
        }
    },
    checkImageResolutions: function (myDocument) {
        try {
            var myLinks = TMUtilities.collectionToArray(myDocument.links);
            var myCount = myLinks.length;
            for (var i = 0; i < myCount; i++) {
                try {
                    var myLink = myLinks[i];
                    var myLinkParent = myLink.parent;
                    if (!myLinkParent) {
                        continue;
                    }
                    try {
                        if (myLink.status == LinkStatus.LINK_MISSING || myLink.status == LinkStatus.LINK_INACCESSIBLE) {
                            continue;
                        }
                    } catch (myException) {
                        continue;
                    }
                    try {
                        var myPpi = myLinkParent.effectivePpi;
                    } catch (myException) {
                        continue;
                    }
                    var myPage = TMPageItem.parentPage(myLinkParent);
                    if (!myPage) {
                        continue;
                    }
                    var myReqResolution = myPage.tmRequiredResolution();
                    if (myPpi[0] < myReqResolution || myPpi[1] < myReqResolution) {
                        var myResolution = myPpi[0].toString();
                        if (myPpi[0] != myPpi[1]) {
                            myResolution += ' x ' + myPpi[1].toString();
                        }
                        this.addWarning(this.reportFullName, 'Link "' + myLink.name + '" on page ' + myPage.name + ' has an effective resolution of ' + Math.ceil(myResolution) + ' ppi where a minimum of ' + Math.ceil(myReqResolution) + ' ppi is suggested.');
                    }
                } catch (myException) { }
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.checkImageResolutions', myException);
        }
    },
    checkBackgroundMusic: function (myDocument) {
        try {
            var properties = myDocument.tmProperties();
            if (!properties.hasOwnProperty('backgroundMusicPlaylist')) {
                return;
            }
            var backgroundMusicPlaylist = properties['backgroundMusicPlaylist'];
            if (!backgroundMusicPlaylist || backgroundMusicPlaylist == '') {
                return;
            }
            backgroundMusicPlaylist = TMFiles.pathRelToAbs(myDocument.filePath.fsName, backgroundMusicPlaylist);
            var backgroundMusicPlaylistFolder = new Folder(backgroundMusicPlaylist);
            if (!backgroundMusicPlaylistFolder.exists) {
                this.addError(this.reportFullName, 'Background playlist folder "' + backgroundMusicPlaylist + '" does not exist for article.');
                return;
            }
            var filesToCopy = backgroundMusicPlaylistFolder.getFiles('*.mp3');
            var totalSize = 0;
            for (var i = 0; i < filesToCopy.length; i++) {
                totalSize += filesToCopy[i].length;
            }
            if (filesToCopy.length == 0 || totalSize == 0) {
                this.addWarning(this.reportFullName, 'Background playlist folder "' + backgroundMusicPlaylist + '" does not contain any files.');
                return;
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.checkBackgroundMusic', myException);
        }
    },
    checkImageSequences: function (myPage) {
        try {
            var reportFullName = this.reportFullName;
            var myPageLayoutName = myPage.tmLayoutName();
            var myPageRectangles = TMUtilities.collectionToArray(myPage.rectangles);
            var myPageRectanglesCount = myPageRectangles.length;
            for (var i = 0; i < myPageRectanglesCount; i++) {
                var myPageItem = myPageRectangles[i];
                var myBaseError = 'Image sequence "' + myPageItem.id + '" on page ' + myPage.name;
                var myLabel = myPageItem.extractLabel("com.rovingbird.epublisher.imagesequence");
                if (myLabel == '') {
                    continue;
                }
                var myProps = TMJSON.parse(myLabel);
                if (!myProps.hasOwnProperty('folder') || myProps.folder == '') {
                    continue;
                }
                var myImageSequenceFolder = undefined;
                var myImageSequenceFolder1 = new Folder(myProps.folder);
                var myImageSequenceFolder2 = new Folder(myPage.tmParentDocument().filePath.fsName + '/' + myProps.folder);
                if (myImageSequenceFolder1.exists) {
                    myImageSequenceFolder = myImageSequenceFolder1;
                } else if (myImageSequenceFolder2.exists) {
                    myImageSequenceFolder = myImageSequenceFolder2;
                } else {
                    this.addError(reportFullName, myBaseError + ' is missing it\'s source folder: "' + myProps.folder + '".');
                    TMLogger.error('Searched ImageSeq in folder: ' + myImageSequenceFolder1);
                    TMLogger.error('Searched ImageSeq in folder: ' + myImageSequenceFolder2);
                    continue;
                }
                var myUsedExtensions = [];
                var myImages = myImageSequenceFolder.getFiles('*.*');
                var myImagesCount = myImages.length;
                for (var j = 0; j < myImagesCount; j++) {
                    if (myImages[j] instanceof File) {
                        var myExt = myImages[j].tmFileExtension().toLowerCase();
                        if (myExt == 'jpg' || myExt == 'jpeg' || myExt == 'png' || myExt == 'gif') {
                            myUsedExtensions.push(myExt);
                        }
                    }
                }
                myUsedExtensions = TMUtilities.uniqueArrayValues(myUsedExtensions);
                if (myUsedExtensions.length == 0) {
                    this.addError(reportFullName, myBaseError + ' does not contain any JPG, PNG or GIF images in the folder: "' + myProps.folder + '".');
                    continue;
                }
                if (myUsedExtensions.length > 1) {
                    this.addError(reportFullName, myBaseError + ' is containing multiple file formats for the images in the folder: \"' + myProps.folder + '\". Please use a single file format for image sequences.');
                    continue;
                }
                if (myPageLayoutName != 'iPad H' && myPageLayoutName != 'iPad V') {
                    var myBounds = TMGeometry.getBounds(myPageItem.geometricBounds);
                    if (myBounds['width'] > 2048) {
                        this.addError(reportFullName, myBaseError + ' has a height bigger than 2048 px which is not supported on Android.');
                        return;
                    }
                    if (myBounds['height'] > 2048) {
                        this.addError(reportFullName, myBaseError + ' has a width bigger than 2048 px which is not supported on Android.');
                        return;
                    }
                }
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.checkImageSequences', myException);
        }
    },
    checkPanoramas: function (myPage) {
        try { } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.checkPanoramas', myException);
        }
    },
    checkRotationAngles: function (myPage) {
        try {
            var reportFullName = this.reportFullName;
            var layoutName = myPage.tmLayoutName();
            var myItems = myPage.tmPageItems();
            var myCount = myItems.length;
            for (var i = 0; i < myCount; i++) {
                var myPageItem = myItems[i];
                if (!TMPageItem.isInteractiveElement(myPageItem)) {
                    continue;
                }
                try {
                    if (myPageItem.rotationAngle != 0) {
                        var name = TMPageItem.analyticsName(myPageItem);
                        var err = 'Item "' + name + '" on page ' + myPage.name + ' in layout "' + layoutName + '" shouldn\'t have a rotation angle applied.';
                        this.addWarning(reportFullName, err);
                    }
                } catch (myException) {
                    TMLogger.error(myException);
                }
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.checkRotationAngles', myException);
        }
    },
    checkAlternateLayoutsInPublication: function (publicationLayouts) {
        try {
            var allPublicationLayouts = [];
            for (var articlePath in publicationLayouts) {
                var articleData = publicationLayouts[articlePath];
                allPublicationLayouts = TMUtilities.mergeArrays(allPublicationLayouts, articleData['layouts']);
            }
            var supportedLayouts = [];
            for (var articlePath in publicationLayouts) {
                var articleData = publicationLayouts[articlePath];
                for (var i = 0; i < allPublicationLayouts.length; i++) {
                    var supportedLayout = allPublicationLayouts[i];
                    if (!TMUtilities.itemInArray(articleData['layouts'], supportedLayout)) {
                        this.addError(articleData['reportName'], 'Article is missing the alternate layout named "' + supportedLayout + '".');
                    }
                }
            }
            return allPublicationLayouts;
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.checkAlternateLayoutNames', myException);
        }
    },
    checkAlternateLayoutNames: function (myDocument) {
        try {
            var myDocumentLayouts = TMAlternateLayouts.documentLayouts(myDocument);
            var mySupportedLayouts = TMExporter.supportedLayouts;
            var hasSupportedLayouts = false;
            for (var i = 0; i < myDocumentLayouts.length; i++) {
                var myDocumentLayout = myDocumentLayouts[i];
                if (TMUtilities.itemInArray(mySupportedLayouts, myDocumentLayout)) {
                    hasSupportedLayouts = true;
                    break;
                }
            }
            for (var i = 0; i < myDocumentLayouts.length; i++) {
                var myDocumentLayout = myDocumentLayouts[i];
                if (!TMUtilities.itemInArray(TMExporter.supportedLayouts, myDocumentLayout)) {
                    this.addError(this.reportFullName, 'Article contains an unsupported alternate layout named "' + myDocumentLayout + '".');
                }
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.checkAlternateLayoutNames', myException);
        }
    },
    checkAlternateLayoutPageCounts: function (myDocument) {
        try {
            var pageCounts = {};
            for (var i = 0; i < myDocument.sections.length; i++) {
                var mySection = myDocument.sections[i];
                var myPageCount = mySection.alternateLayoutLength;
                var myLayout = mySection.tmLayoutName();
                myLayout = myLayout.substr(0, myLayout.length - 2);
                if (myLayout != '') {
                    if (pageCounts[myLayout] == undefined) {
                        pageCounts[myLayout] = [];
                    }
                    pageCounts[myLayout].push(myPageCount);
                }
            }
            for (var layout in pageCounts) {
                var pageCount = pageCounts[layout];
                if (TMUtilities.uniqueArrayValues(pageCount).length > 1) {
                    this.addError(this.reportFullName, 'Article contains different page counts for both orientations in the ' + layout + ' layout.');
                }
            };
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.checkAlternateLayoutPageCounts', myException);
        }
    },
    checkMissingFonts: function (myDocument) {
        try {
            var myDocumentFonts = TMUtilities.collectionToArray(myDocument.fonts);
            var myDocumentFontsCount = myDocumentFonts.length;
            for (var i = 0; i < myDocumentFontsCount; i++) {
                var myFont = myDocumentFonts[i];
                try {
                    var myFontName = myFont.name;
                    var myFontStatus = myFont.status;
                    if (myFontStatus == FontStatus.NOT_AVAILABLE) {
                        this.addWarning(this.reportFullName, 'Font "' + myFontName + '" is missing.');
                    }
                } catch (myException) { }
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.checkMissingFonts', myException);
        }
    },
    checkMissingLinks: function (myDocument) {
        try {
            var myDocumentLinks = TMUtilities.collectionToArray(myDocument.links);
            var myDocumentLinksCount = myDocumentLinks.length;
            for (var i = 0; i < myDocumentLinksCount; i++) {
                var myLink = myDocumentLinks[i];
                try {
                    var myLinkName = myLink.name;
                    var myLinkStatus = myLink.status;
                    if (myLinkStatus == LinkStatus.LINK_MISSING) {
                        this.addWarning(this.reportFullName, 'Link "' + myLinkName + '" is missing.');
                    } else if (myLinkStatus == LinkStatus.LINK_INACCESSIBLE) {
                        this.addWarning(this.reportFullName, 'Link "' + myLinkName + '" is inaccessible.');
                    }
                } catch (myException) { }
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.checkMissingLinks', myException);
        }
    },
    checkFacingPages: function (myDocument) {
        try {
            if (myDocument.documentPreferences.facingPages) {
                this.addError(this.reportFullName, 'This document uses facing pages which is not supported.');
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMPreflighter.checkFacingPages', myException);
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMProgressBar = {
    progress: undefined,
    create: function (myTitle, myLabel, myMax) {
        try {
            TMPreflighter.reset();
            if (TMApplication.isServer()) {
                return;
            }
            var myRes = "palette { alignChildren: 'fill', text: '" + myTitle.replace('\'', '\\\'') + "', \
label:       StaticText { text: '" + myLabel.replace('\'', '\\\'') + "' }, \
subLabel:    StaticText { text: '' }, \
progressBar: Progressbar { minvalue: 0, maxvalue: " + myMax + " } \
}";
            this.progress = new Window(myRes, myTitle, undefined, {
                resizable: false,
                closeButton: false
            });
            if (myMax == 0) {
                this.progress.progressBar.visible = false;
            } else {
                this.progress.progressBar.visible = true;
            }
            this.progress.preferredSize.width = 440;
            this.progress.center();
            this.progress.show();
        } catch (myException) {
            TMLogger.exception("TMProgressBar.create", myException + ' (tm_progressbar:36)');
        }
    },
    updateTitle: function (myTitleText) {
        try {
            if (TMApplication.isServer()) {
                return;
            }
            if (this.progress == undefined) {
                return;
            }
            this.progress.text = myTitleText;
        } catch (myException) {
            TMLogger.exception("TMProgressBar.updateTitle", myException + ' (tm_progressbar:54)');
        }
    },
    update: function (myLabelText, mySubLabelText, myProgress) {
        try {
            if (TMApplication.isServer()) {
                return;
            }
            if (this.progress == undefined) {
                return;
            }
            if (myLabelText != undefined) {
                this.progress.label.text = myLabelText;
            }
            if (mySubLabelText != undefined) {
                this.progress.subLabel.text = mySubLabelText;
            }
            if (myProgress != undefined) {
                if (myProgress > this.progress.progressBar.maxvalue) {
                    myProgress = this.progress.progressBar.maxvalue;
                }
                this.progress.progressBar.value = myProgress;
            }
            try {
                this.progress.update();
            } catch (myException) { }
        } catch (myException) {
            TMLogger.silentException("TMProgressBar.update", myException);
        }
    },
    updateLabel: function (myLabelText, mySubLabelText, myProgress) {
        try {
            this.update(myLabelText, mySubLabelText, myProgress);
        } catch (myException) {
            TMLogger.exception("TMProgressBar.updateLabel", myException + ' (tm_progressbar:94)');
        }
    },
    updateSubLabel: function (mySubLabelText, myLabelText, myProgress) {
        try {
            this.update(myLabelText, mySubLabelText, myProgress);
        } catch (myException) {
            TMLogger.exception("TMProgressBar.updateSubLabel", myException + ' (tm_progressbar:102)');
        }
    },
    updateProgress: function (myProgress, myLabelText, mySubLabelText) {
        try {
            this.update(myLabelText, mySubLabelText, myProgress);
        } catch (myException) {
            TMLogger.exception("TMProgressBar.updateProgress", myException + ' (tm_progressbar:110)');
        }
    },
    updateProgressBySteps: function (numberOfSteps) {
        try {
            if (TMApplication.isServer()) {
                return;
            }
            if (this.progress != undefined) {
                this.progress.progressBar.value += numberOfSteps;
                this.update();
            }
        } catch (myException) {
            TMLogger.exception("TMProgressBar.updateProgressBySteps", myException + ' (tm_progressbar:127)');
        }
    },
    close: function () {
        try {
            if (TMApplication.isServer()) {
                return;
            }
            if (this.progress != undefined) {
                this.progress.close();
                this.progress = undefined;
            }
        } catch (myException) {
            TMLogger.exception("TMProgressBar.close", myException + ' (tm_progressbar:144)');
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMPublicationWriter = {
    suggestedShowStatusBar: function (options, data) {
        try {
            if (TMPreferences.isRunningLegacyMode() == false) {
                return false;
            }
            var showStatusBar = TMUtilities.getBoolProperty(options, 'showStatusBar', 'no');
            var layout = TMUtilities.getProperty(options, 'layout', undefined);
            var screenHeightLandscape = parseInt(layout.split('x')[1]);
            var screenHeightPortrait = parseInt(layout.split('x')[0]);
            for (var myArticleName in data) {
                if (data[myArticleName]['landscape'] != undefined) {
                    var myArticleData = data[myArticleName]['landscape'];
                    if (myArticleData.pages) {
                        for (var myPageId in myArticleData.pages) {
                            myPage = myArticleData.pages[myPageId];
                            if (myPage['height'] < screenHeightLandscape) {
                                TMLogger.info('Found a page with a height of ' + myPage['height'] + ' px, enabling the status bar');
                                return true;
                            }
                        }
                    }
                }
                if (data[myArticleName]['portrait'] != undefined) {
                    var myArticleData = data[myArticleName]['portrait'];
                    if (myArticleData.pages) {
                        for (var myPageId in myArticleData.pages) {
                            myPage = myArticleData.pages[myPageId];
                            if (myPage['height'] < screenHeightPortrait) {
                                TMLogger.info('Found a page with a height of ' + myPage['height'] + ' px, enabling the status bar');
                                return true;
                            }
                        }
                    }
                }
            }
            showStatusBar = false;
            TMLogger.info('Using the default value for the status bar: ' + showStatusBar);
            return showStatusBar;
        } catch (myException) {
            TMStackTrace.addToStack("TMPublicationWriter.suggestedShowStatusBar", myException);
        }
    },
    writePublication: function (options, data, file) {
        try {
            var scrubber = TMUtilities.getBoolProperty(options, 'scrubber', 'yes');
            var scrollEnabled = TMUtilities.getBoolProperty(options, 'scrollEnabled', 'yes');
            var jpegQuality = TMUtilities.getProperty(options, 'jpegQuality', 'High');
            var showStatusBar = this.suggestedShowStatusBar(options, data);
            var horizontalOnly = TMUtilities.getBoolProperty(options, 'horizontalOnly', 'no');
            var allowSharing = TMUtilities.getBoolProperty(options, 'allowSharing', 'yes');
            var version = TMVersion.version;
            var build = TMVersion.build;
            var layout = TMUtilities.getProperty(options, 'layout', undefined);
            var readingStyle = TMUtilities.getProperty(options, 'readingStyle', 'Left to Right');
            var indesignVersion = TMPluginCore.appDisplayVersion() + '/' + File.fs.toLowerCase().substr(0, 3);
            if (layout == '568x320' || layout == '667x375' || layout == '736x414') {
                showStatusBar = false;
            }
            var orientations = [];
            for (var myArticleName in data) {
                var myArticle = data[myArticleName];
                orientations = TMUtilities.objectKeys(myArticle);
            }
            if (TMUtilities.itemInArray(orientations, 'portrait') && TMUtilities.itemInArray(orientations, 'landscape')) {
                orientations = 'both';
            } else if (TMUtilities.itemInArray(orientations, 'portrait')) {
                orientations = 'portrait';
            } else if (TMUtilities.itemInArray(orientations, 'landscape')) {
                orientations = 'landscape';
            }
            var publicationProperties = {
                name: options['name'],
                orientations: orientations,
                scrubber: scrubber,
                jpegQuality: jpegQuality,
                scrollEnabled: scrollEnabled,
                showStatusBar: showStatusBar,
                horizontalOnly: horizontalOnly,
                version: version,
                build: build,
                allowSharing: allowSharing,
                dataModel: 3,
                layout: layout,
                creator: indesignVersion,
                readingStyle: readingStyle.replace(/\s+/g, '-').toLowerCase(),
                exportFormat: kEXPORT_FORMAT,
            };
            if (scrubber == true || scrubber == 'yes') {
                publicationProperties['scrubberStyle'] = 2;
            }
            var myXml = new File(file);
            myXml.encoding = "UTF-8";
            myXml.open('w');
            myXml.write("\uFEFF");
            myXml.writeln('<?xml version="1.0" encoding="UTF-8"?>');
            TMXml.writeTag(myXml, 'publication', publicationProperties);
            for (var myArticleName in data) {
                myArticleName = myArticleName.replace(/\r?\n/g, ' ');
                var myArticleShowInScrubber = data[myArticleName]['showInScrubber'];
                var backgroundMusicPlaylist = data[myArticleName]['backgroundMusicPlaylist'];
                TMXml.startTag(myXml, 'article', {
                    showInScrubber: myArticleShowInScrubber,
                    backgroundMusicPlaylist: backgroundMusicPlaylist
                });
                TMXml.writeTextTag(myXml, 'name', myArticleName);
                if (data[myArticleName]['landscape'] != undefined) {
                    this.writeArticle(
                        options, myXml, myArticleName, data[myArticleName]['landscape'], 'landscape', scrubber, myArticleShowInScrubber
                    );
                }
                if (data[myArticleName]['portrait'] != undefined) {
                    this.writeArticle(
                        options, myXml, myArticleName, data[myArticleName]['portrait'], 'portrait', scrubber, myArticleShowInScrubber
                    );
                }
                TMXml.closeTag(myXml, 'article');
            }
            TMXml.closeTag(myXml, 'publication');
            myXml.close();
            XML.prettyIndent = 4;
            XML.prettyPrinting = true;
            var myXml = TMFiles.readUTF8DataFromFile(file);
            var myFormattedXml = '<?xml version="1.0" encoding="UTF-8"?>\n' + new XML(myXml).toXMLString();
            if (kEXPORT_FORMAT == 'PDF') {
                myFormattedXml = myFormattedXml.replace(new RegExp('568x320', 'g'), '736x414');
            }
            TMFiles.writeUTF8DataToFile(file, myFormattedXml);
            this.writePublicationJson(data, file);
        } catch (myException) {
            TMStackTrace.addToStack("TMPublicationWriter.writePublication", myException);
        }
    },
    writePublicationJson: function (data, file) {
        try {
            var myArticles = [];
            for (var a in data) {
                var myArticle = data[a];
                myArticle['name'] = a;
                if (myArticle['landscape']) {
                    if (myArticle['landscape']['pagecount']) {
                        delete myArticle['landscape']['pagecount'];
                    }
                    var pages = []
                    if (myArticle['landscape']['pages']) {
                        for (var i in myArticle['landscape']['pages']) {
                            var page = myArticle['landscape']['pages'][i];
                            delete page['actions'];
                            delete page['full'];
                            delete page['height'];
                            delete page['id'];
                            delete page['imagesequences'];
                            delete page['movies'];
                            delete page['multistates'];
                            delete page['pagelinks'];
                            delete page['scrollables'];
                            delete page['sounds'];
                            delete page['weblinks'];
                            delete page['weboverlays'];
                            delete page['webviewers'];
                            delete page['width'];
                            delete page['x'];
                            delete page['y'];
                            delete page['showInScrubber'];
                            pages.push(page);
                        }
                    }
                    myArticle['landscape']['pages'] = pages;
                }
                if (myArticle['portrait']) {
                    if (myArticle['portrait']['pagecount']) {
                        delete myArticle['portrait']['pagecount'];
                    }
                    var pages = []
                    if (myArticle['portrait']['pages']) {
                        for (var i in myArticle['portrait']['pages']) {
                            var page = myArticle['portrait']['pages'][i];
                            delete page['actions'];
                            delete page['full'];
                            delete page['height'];
                            delete page['id'];
                            delete page['imagesequences'];
                            delete page['movies'];
                            delete page['multistates'];
                            delete page['pagelinks'];
                            delete page['scrollables'];
                            delete page['sounds'];
                            delete page['weblinks'];
                            delete page['weboverlays'];
                            delete page['webviewers'];
                            delete page['width'];
                            delete page['x'];
                            delete page['y'];
                            delete page['showInScrubber'];
                            pages.push(page);
                        }
                    }
                    myArticle['portrait']['pages'] = pages;
                }
                myArticles.push(myArticle);
            }
            var jsonPath = file.replace('.xml', '.json');
            var jsonData = TMJSON.stringify(myArticles);
            if (kEXPORT_FORMAT == 'PDF') {
                jsonData = jsonData.replace(new RegExp('568x320', 'g'), '736x414');
            }
            TMFiles.writeUTF8DataToFile(jsonPath, jsonData);
        } catch (myException) {
            TMStackTrace.addToStack("TMPublicationWriter.writePublicationJson", myException);
        }
    },
    writeArticle: function (myOptions, myXml, myArticleName, myArticleData, myOrientation, myScrubber, myArticleShowInScrubber) {
        try {
            TMXml.startTag(myXml, myOrientation);
            TMXml.writeTextTag(myXml, 'title', myArticleData['title']);
            TMXml.writeTextTag(myXml, 'tagline', myArticleData['tagline']);
            TMXml.writeTextTag(myXml, 'author', myArticleData['author']);
            TMXml.writeTextTag(myXml, 'articleURL', myArticleData['articleURL']);
            TMXml.writeTag(myXml, 'pages', {
                count: myArticleData['pageCount']
            });
            if (myArticleData.pages) {
                for (var myPageId in myArticleData.pages) {
                    myPage = myArticleData.pages[myPageId];
                    if (myPage['showInScrubber'] == undefined) {
                        myPage['showInScrubber'] = true;
                    }
                    TMXml.writeTag(myXml, 'page', {
                        number: myPage['number'],
                        width: myPage['width'],
                        height: myPage['height'],
                        color: myPage['color']
                    });
                    TMXml.writeTextTag(myXml, 'full', myPage['full']);
                    TMXml.writeTextTag(myXml, 'thumb', myPage['thumb']);
                    TMXml.writeArray(myXml, myPage['webviewers'], 'webviewers', 'webviewer');
                    TMXml.writeArray(myXml, myPage['weboverlays'], 'weboverlays', 'weboverlay');
                    TMXml.writeArray(myXml, myPage['weblinks'], 'weblinks', 'weblink');
                    TMXml.writeArray(myXml, myPage['pagelinks'], 'pagelinks', 'pagelink');
                    TMXml.writeArray(myXml, myPage['movies'], 'movies', 'movie');
                    TMXml.writeArray(myXml, myPage['sounds'], 'sounds', 'sound');
                    this.writeActions(myXml, myPage['actions']);
                    TMXml.writeArray(myXml, myPage['imagesequences'], 'imagesequences', 'imagesequence');
                    if (myPage['scrollables'] && myPage['scrollables'].length > 0) {
                        TMXml.startTag(myXml, 'scrollables');
                        var myCount = myPage['scrollables'].length;
                        for (var i = 0; i < myCount; i++) {
                            var myScrollable = myPage['scrollables'][i];
                            TMXml.writeTag(myXml, 'scrollable', myScrollable);
                            this.writeActions(myXml, myScrollable['actions']);
                            TMXml.writeArray(myXml, myScrollable['pagelinks'], 'pagelinks', 'pagelink');
                            TMXml.writeArray(myXml, myScrollable['weblinks'], 'weblinks', 'weblink');
                            TMXml.writeArray(myXml, myScrollable['movies'], 'movies', 'movie');
                            TMXml.writeArray(myXml, myScrollable['sounds'], 'sounds', 'sound');
                            TMXml.writeArray(myXml, myScrollable['weboverlays'], 'weboverlays', 'weboverlay');
                            TMXml.writeArray(myXml, myScrollable['webviewers'], 'webviewers', 'webviewer');
                            TMXml.closeTag(myXml, "scrollable");
                        }
                        TMXml.closeTag(myXml, 'scrollables');
                    }
                    if (myPage['multistates'] && myPage['multistates'].length > 0) {
                        TMXml.startTag(myXml, 'multistates');
                        var myCount = myPage['multistates'].length;
                        for (var i = 0; i < myCount; i++) {
                            var mySlideShow = myPage['multistates'][i];
                            var mySlidesCount = mySlideShow['slides'].length;
                            if (mySlidesCount == 0) {
                                continue;
                            }
                            TMXml.writeTag(myXml, 'multistate', mySlideShow);
                            for (var j = 0; j < mySlidesCount; j++) {
                                var mySlide = mySlideShow['slides'][j];
                                TMXml.writeTag(myXml, 'state', mySlide);
                                this.writeActions(myXml, mySlide['actions']);
                                TMXml.writeArray(myXml, mySlide['pagelinks'], 'pagelinks', 'pagelink');
                                TMXml.writeArray(myXml, mySlide['weblinks'], 'weblinks', 'weblink');
                                TMXml.writeArray(myXml, mySlide['movies'], 'movies', 'movie');
                                TMXml.writeArray(myXml, mySlide['sounds'], 'sounds', 'sound');
                                TMXml.writeArray(myXml, mySlide['weboverlays'], 'weboverlays', 'weboverlay');
                                TMXml.writeArray(myXml, mySlide['webviewers'], 'webviewers', 'webviewer');
                                TMXml.closeTag(myXml, 'state');
                            }
                            TMXml.closeTag(myXml, "multistate");
                        }
                        TMXml.closeTag(myXml, 'multistates');
                    }
                    TMXml.closeTag(myXml, "page");
                }
            }
            TMXml.closeTag(myXml, "pages");
            TMXml.closeTag(myXml, myOrientation);
        } catch (myException) {
            TMStackTrace.addToStack("TMPublicationWriter.writeArticle", myException);
        }
    },
    writeActions: function (myXml, myActions) {
        if (!myActions || myActions.length == 0) {
            return;
        }
        var allButtons = [];
        var allActions = [];
        for (var i in myActions) {
            var myButton = myActions[i];
            if (myButton['actions'].length == 1 && !myButton['normal'] && !myButton['click'] && myButton['actions'][0]['action'] != 'pagelink') {
                allActions.push(myButton);
            } else if (myButton['actions'].length == 1) {
                allButtons.push(myButton);
            } else {
                allButtons.push(myButton);
            }
        }
        if (allButtons.length > 0) {
            TMXml.startTag(myXml, 'buttons');
            for (var i in allButtons) {
                var myButton = allButtons[i];
                var buttonProperties = {
                    x: myButton['x'],
                    y: myButton['y'],
                    width: myButton['width'],
                    height: myButton['height']
                };
                if (myButton['normal']) {
                    buttonProperties['normal'] = myButton['normal'];
                }
                if (myButton['click']) {
                    buttonProperties['click'] = myButton['click'];
                }
                TMXml.writeTag(myXml, 'button', buttonProperties);
                for (var j in myButton['actions']) {
                    var myAction = myButton['actions'][j];
                    delete myAction['x'];
                    delete myAction['y'];
                    delete myAction['width'];
                    delete myAction['height'];
                    TMXml.writeTag(myXml, 'action', myAction, true);
                }
                TMXml.closeTag(myXml, 'button');
            }
            TMXml.closeTag(myXml, 'buttons');
        }
        if (allActions.length > 0) {
            TMXml.startTag(myXml, 'actions');
            for (var i in allActions) {
                var myButton = allActions[i];
                var myAction = {
                    x: myButton['x'],
                    y: myButton['y'],
                    width: myButton['width'],
                    height: myButton['height']
                };
                if (myButton['actions'].length == 1) {
                    for (var key in myButton['actions'][0]) {
                        myAction[key] = myButton['actions'][0][key];
                    }
                    TMXml.writeTag(myXml, 'action', myAction, true);;
                }
            }
            TMXml.closeTag(myXml, 'actions');
        }
    },
    rewritePublication: function (myXmlPath, srcLayout, dstLayout) {
        try {
            var myXmlFile = new File(myXmlPath);
            TMHelper.call('publication/rewrite', {
                'xmlPath': myXmlFile.fsName,
                'srcLayout': srcLayout,
                'dstLayout': dstLayout,
            });
        } catch (myException) {
            TMStackTrace.addToStack("TMPublicationWriter.rewritePublication", myException);
        }
    },
    rewritePublicationXml: function (mySrcFile, myDstFile, srcLayout, dstLayout) {
        try {
            TMLogger.info('Creating: ' + myDstFile.fsName);
            var scalingFactor = parseFloat(this.rewriteScalingFactor(srcLayout, dstLayout));
            TMLogger.debug('Applying scaling factor: ' + scalingFactor);
            XML.prettyIndent = 4;
            XML.prettyPrinting = true;
            var xml = XML(TMFiles.readUTF8DataFromFile(mySrcFile.fsName));
            xml.@layout = dstLayout;
            for (var i = 0; i < xml.descendants().length(); i++) {
                var node = xml.descendants()[i];
                node = this.rewriteNodeWidthScalingFactor(node, scalingFactor);
            }
            var xml = '<?xml version="1.0" encoding="UTF-8"?>\n' + xml.toXMLString();
            if (kEXPORT_FORMAT == 'JPG') {
                xml = xml.replace(new RegExp(srcLayout, 'g'), dstLayout);
            }
            TMFiles.writeUTF8DataToFile(myDstFile.fsName, xml);
        } catch (myException) {
            TMStackTrace.addToStack("TMPublicationWriter.rewritePublicationXml", myException);
        }
    },
    rewritePublicationJson: function (mySrcFile, myDstFile, srcLayout, dstLayout) {
        try {
            var json = TMFiles.readUTF8DataFromFile(mySrcFile.fsName);
            json = json.replace(new RegExp(srcLayout, 'g'), dstLayout);
            TMFiles.writeUTF8DataToFile(myDstFile.fsName, json);
        } catch (myException) {
            TMStackTrace.addToStack("TMPublicationWriter.rewritePublicationJson", myException);
        }
    },
    rewriteScalingFactor: function (srcLayout, dstLayout) {
        try {
            var srcWidth = parseInt(srcLayout.split('x')[0]);
            var dstWidth = parseInt(dstLayout.split('x')[0]);
            return dstWidth / srcWidth;
        } catch (myException) {
            TMStackTrace.addToStack("TMPublicationWriter.rewriteScalingFactor", myException);
        }
    },
    rewriteNodeWidthScalingFactor: function (node, scalingFactor) {
        try {
            if (node.@width != undefined) {
                node.@width = this.rewriteValueWithScalingFactor(node.@width, scalingFactor);
            }
            if (node.@height != undefined) {
                node.@height = this.rewriteValueWithScalingFactor(node.@height, scalingFactor);
            }
            if (node.@w != undefined) {
                node.@w = this.rewriteValueWithScalingFactor(node.@w, scalingFactor);
            }
            if (node.@h != undefined) {
                node.@h = this.rewriteValueWithScalingFactor(node.@h, scalingFactor);
            }
            if (node.@x != undefined) {
                node.@x = this.rewriteValueWithScalingFactor(node.@x, scalingFactor);
            }
            if (node.@y != undefined) {
                node.@y = this.rewriteValueWithScalingFactor(node.@y, scalingFactor);
            }
            if (node.@xpopup != undefined) {
                node.@xpopup = this.rewriteValueWithScalingFactor(node.@xpopup, scalingFactor);
            }
            if (node.@ypopup != undefined) {
                node.@ypopup = this.rewriteValueWithScalingFactor(node.@ypopup, scalingFactor);
            }
            if (node.@wpopup != undefined) {
                node.@wpopup = this.rewriteValueWithScalingFactor(node.@wpopup, scalingFactor);
            }
            if (node.@hpopup != undefined) {
                node.@hpopup = this.rewriteValueWithScalingFactor(node.@hpopup, scalingFactor);
            }
            if (node.@contentX != undefined) {
                node.@contentX = this.rewriteValueWithScalingFactor(node.@contentX, scalingFactor);
            }
            if (node.@contentY != undefined) {
                node.@contentY = this.rewriteValueWithScalingFactor(node.@contentY, scalingFactor);
            }
            if (node.@contentWidth != undefined) {
                node.@contentWidth = this.rewriteValueWithScalingFactor(node.@contentWidth, scalingFactor);
            }
            if (node.@contentHeight != undefined) {
                node.@contentHeight = this.rewriteValueWithScalingFactor(node.@contentHeight, scalingFactor);
            }
        } catch (myException) {
            TMStackTrace.addToStack("TMPublicationWriter.rewriteNodeWidthScalingFactor", myException);
        }
    },
    rewriteValueWithScalingFactor: function (value, scalingFactor) {
        try {
            return parseFloat(parseFloat(parseFloat(value) * scalingFactor).toFixed(kPRECISION));
        } catch (myException) {
            TMStackTrace.addToStack("TMPublicationWriter.rewriteValueWithScalingFactor", myException);
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
Section.prototype.tmPages = function () {
    try {
        var myDocument = this.parent;
        var myStartPage = this.pageStart.documentOffset;
        var myPageCount = this.length;
        var myPages = [];
        var myDocumentPages = TMUtilities.collectionToArray(myDocument.pages);
        for (var i = myStartPage; i < myStartPage + myPageCount; i++) {
            var myPage = myDocumentPages[i];
            myPages.push(myPage);
        };
        return myPages;
    } catch (myException) {
        TMLogger.exception("Section.prototype.tmPages", myException + ' (tm_section:30)');
        return [];
    }
};
Section.prototype.tmLastSectionPage = function () {
    try {
        var mySectionPages = this.tmPages();
        return mySectionPages.pop();
    } catch (myException) {
        TMLogger.exception("Section.prototype.tmLastSectionPage", myException + ' (tm_section:40)');
        return undefined;
    }
};
Section.prototype.tmOrientation = function () {
    try {
        return this.pageStart.tmOrientation();
    } catch (myException) {
        TMStackTrace.addToStack("Section.prototype.tmOrientation", myException);
    }
};
Section.prototype.tmLayoutName = function () {
    try {
        if (TMPreferences.isRunningLegacyMode() == false) {
            return this.alternateLayout;
        }
        if (this.parent.tmUsesAlternateLayouts()) {
            return this.alternateLayout;
        }
    } catch (myException) { }
    var mySuffix = (this.tmOrientation() == 'portrait') ? 'V' : 'H';
    return 'iPad ' + mySuffix;
};
Section.prototype.tmLayoutDimensions = function (myLayoutName) {
    try {
        if (!myLayoutName) {
            myLayoutName = this.tmLayoutName();
        }
        if (myLayoutName.tmStartsWith('Android 10" ')) {
            return '1280x800';
        } else if (myLayoutName.tmStartsWith('Kindle Fire/Nook ')) {
            return '1024x600';
        } else if (myLayoutName.tmStartsWith('Phone ')) {
            return '568x320';
        } else {
            return '1024x768';
        }
    } catch (myException) {
        TMLogger.exception("Section.prototype.tmLayoutDimensions", myException + ' (tm_section:82)');
        return '1024x768';
    }
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMSetup = {
    exportFullPDF: function (myResolution) {
        try {
            TMLogger.debug('    Setting up PDF export using resolution: ' + myResolution + ' ppi');
            app.pdfExportPreferences.acrobatCompatibility = AcrobatCompatibility.ACROBAT_8;
            app.pdfExportPreferences.bleedBottom = 0;
            app.pdfExportPreferences.bleedInside = 0;
            app.pdfExportPreferences.bleedMarks = false;
            app.pdfExportPreferences.bleedOutside = 0;
            app.pdfExportPreferences.bleedTop = 0;
            app.pdfExportPreferences.colorBars = false;
            app.pdfExportPreferences.colorBitmapCompression = BitmapCompression.JPEG;
            app.pdfExportPreferences.colorBitmapQuality = CompressionQuality.HIGH;
            app.pdfExportPreferences.colorBitmapSampling = Sampling.BICUBIC_DOWNSAMPLE;
            app.pdfExportPreferences.colorBitmapSamplingDPI = Math.ceil(myResolution);
            app.pdfExportPreferences.compressTextAndLineArt = true;
            app.pdfExportPreferences.compressionType = PDFCompressionType.COMPRESS_STRUCTURE;
            app.pdfExportPreferences.cropImagesToFrames = true;
            app.pdfExportPreferences.cropMarks = false;
            app.pdfExportPreferences.exportGuidesAndGrids = false;
            app.pdfExportPreferences.exportLayers = false;
            app.pdfExportPreferences.exportNonprintingObjects = false;
            app.pdfExportPreferences.exportReaderSpreads = false;
            app.pdfExportPreferences.exportWhichLayers = ExportLayerOptions.EXPORT_VISIBLE_PRINTABLE_LAYERS;
            app.pdfExportPreferences.generateThumbnails = false;
            app.pdfExportPreferences.grayscaleBitmapCompression = BitmapCompression.JPEG;
            app.pdfExportPreferences.grayscaleBitmapQuality = CompressionQuality.HIGH;
            app.pdfExportPreferences.grayscaleBitmapSampling = Sampling.BICUBIC_DOWNSAMPLE;
            app.pdfExportPreferences.grayscaleBitmapSamplingDPI = Math.ceil(myResolution);
            app.pdfExportPreferences.includeBookmarks = false;
            app.pdfExportPreferences.includeHyperlinks = false;
            app.pdfExportPreferences.includeSlugWithPDF = false;
            app.pdfExportPreferences.includeStructure = false;
            app.pdfExportPreferences.interactiveElementsOption = InteractiveElementsOptions.APPEARANCE_ONLY;
            app.pdfExportPreferences.monochromeBitmapCompression = MonoBitmapCompression.CCIT4;
            app.pdfExportPreferences.monochromeBitmapSampling = Sampling.BICUBIC_DOWNSAMPLE;
            app.pdfExportPreferences.monochromeBitmapSamplingDPI = Math.ceil(myResolution);
            app.pdfExportPreferences.omitBitmaps = false;
            app.pdfExportPreferences.omitEPS = false;
            app.pdfExportPreferences.omitPDF = false;
            app.pdfExportPreferences.optimizePDF = false;
            app.pdfExportPreferences.pageInformationMarks = false;
            app.pdfExportPreferences.pageMarksOffset = 0;
            app.pdfExportPreferences.pdfColorSpace = PDFColorSpace.RGB;
            app.pdfExportPreferences.pdfMarkType = MarkTypes.DEFAULT_VALUE;
            app.pdfExportPreferences.printerMarkWeight = PDFMarkWeight.P125PT;
            app.pdfExportPreferences.registrationMarks = false;
            app.pdfExportPreferences.standardsCompliance = PDFXStandards.NONE;
            app.pdfExportPreferences.subsetFontsBelow = 100;
            app.pdfExportPreferences.thresholdToCompressColor = Math.ceil(myResolution * 1.2);
            app.pdfExportPreferences.thresholdToCompressGray = Math.ceil(myResolution * 1.2);
            app.pdfExportPreferences.thresholdToCompressMonochrome = Math.ceil(myResolution * 1.2);
            app.pdfExportPreferences.useDocumentBleedWithPDF = false;
            app.pdfExportPreferences.useSecurity = false;
            app.pdfExportPreferences.viewPDF = false;
            app.pdfExportPreferences.includeICCProfiles = false;
            app.pdfExportPreferences.pdfDestinationProfile = 'sRGB IEC61966-2.1';
            try {
                app.pdfExportPreferences.exportAsSinglePages = false;
            } catch (ignoredException) { }
        } catch (myException) {
            TMLogger.exception("TMSetup.exportFullPDF", myException + ' (tm_setup:71)');
        }
    },
    exportFullPNG: function () {
        try {
            app.pngExportPreferences.antiAlias = true;
            app.pngExportPreferences.pngColorSpace = PNGColorSpaceEnum.RGB;
            app.pngExportPreferences.exportingSpread = false;
            app.pngExportPreferences.pngExportRange = ExportRangeOrAllPages.EXPORT_RANGE;
            app.pngExportPreferences.exportResolution = Math.ceil(gBaseResolution);
            app.pngExportPreferences.simulateOverprint = false; // if true, bad output quality, needs to be on for GWG test files
            app.pngExportPreferences.useDocumentBleeds = false;
            app.pngExportPreferences.transparentBackground = true;
            app.pngExportPreferences.pngQuality = PNGQualityEnum.MAXIMUM;
        } catch (myException) {
            TMLogger.exception("TMSetup.exportFullPNG", myException + ' (tm_setup:87)');
        }
    },
    exportFullJPG: function () {
        try {
            app.jpegExportPreferences.antiAlias = true;
            app.jpegExportPreferences.jpegColorSpace = JpegColorSpaceEnum.RGB;
            app.jpegExportPreferences.exportingSpread = false;
            app.jpegExportPreferences.jpegExportRange = ExportRangeOrAllPages.EXPORT_RANGE;
            app.jpegExportPreferences.jpegRenderingStyle = JPEGOptionsFormat.BASELINE_ENCODING;
            app.jpegExportPreferences.exportResolution = Math.ceil(gBaseResolution * 2);
            app.jpegExportPreferences.simulateOverprint = false; // if true, bad output quality, needs to be on for GWG test files
            app.jpegExportPreferences.useDocumentBleeds = false;
            app.jpegExportPreferences.embedColorProfile = false;
            var myJpegQuality = JPEGOptionsQuality.HIGH;
            if (app.books.length > 0) {
                var myQuality = app.activeBook.tmExportQuality(); // Shortcut, I know, but this was the fastest solution
                if (myQuality == 'maximum') {
                    myJpegQuality = JPEGOptionsQuality.MAXIMUM;
                } else if (myQuality == 'medium') {
                    myJpegQuality = JPEGOptionsQuality.MEDIUM;
                } else if (myQuality == 'low') {
                    myJpegQuality = JPEGOptionsQuality.LOW;
                } else {
                    myJpegQuality = JPEGOptionsQuality.HIGH;
                }
            }
            app.jpegExportPreferences.jpegQuality = myJpegQuality;
        } catch (myException) {
            TMLogger.exception("TMSetup.exportFullJPG", myException + ' (tm_setup:120)');
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMStackTrace = {
    stackTrace: [],
    addToStack: function (functionName, myException) {
        TMStackTrace.stackTrace.push(functionName);
        throw myException;
    },
    getStackTrace: function (myException) {
        try {
            var count = TMStackTrace.stackTrace.length;
            var stackTrace = [];
            for (var i = 0; i < TMStackTrace.stackTrace.length; i++) {
                stackTrace.push('' + i + ': ' + TMStackTrace.stackTrace[i]);
            }
            TMStackTrace.resetStackTrace();
            return stackTrace.join('\n');
        } catch (e) {
            TMLogger.e('TMStackTrace.addToStack', e);
            throw e;
        }
    },
    resetStackTrace: function () {
        TMStackTrace.stackTrace = [];
    },
}
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMSupport = {
    exportSupportLogs: function () {
        if (!TMChecks.isHelperRunning()) {
            return;
        }
        var exportPath = TMFiles.saveFile(this.getExportFileName(), 'Export support logfiles as:');
        if (!exportPath) {
            return;
        }
        TwixlPublisherPluginAPI.exportSupportLogs(exportPath);
    },
    getExportFileName: function () {
        var date = new Date();
        var year = date.getFullYear();
        var month = ('00' + (date.getMonth() + 1)).substr(-2);
        var day = ('00' + date.getDate()).substr(-2);
        var hours = ('00' + date.getHours()).substr(-2);
        var minutes = ('00' + date.getMinutes()).substr(-2);
        var seconds = ('00' + date.getSeconds()).substr(-2);
        return Folder.desktop.fsName + '/twixl_logfiles_' + year + month + +day + '_' + hours + '' + minutes + '.zip';
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMTaskQueue = {
    getTaskFolderPath: function () {
        if (File.fs == 'Macintosh') {
            return '/Library/Application Support/Twixl Publisher/Queue';
        } else {
            return Folder.appData.fsName + '\\Twixl Publisher\\Queue';
        }
    },
    createFolderIfNeeded: function () {
        var taskFolder = new Folder(this.getTaskFolderPath());
        if (!taskFolder.parent.exists) {
            TMLogger.info('Creating task queue folder: ' + taskFolder.parent.fsName);
            taskFolder.parent.create();
        }
        if (!taskFolder.exists) {
            TMLogger.info('Creating task queue folder: ' + taskFolder.fsName);
            taskFolder.create();
        }
    },
    addTask: function (filePath, params) {
        try {
            if (!params) {
                params = {}
            }
            var taskId = Math.round(new Date().getTime() / 1000) + '_' + TMUtilities.uuid().replace(/-/g, '');
            if (filePath) {
                var file = new File(filePath);
                if (!file.exists) {
                    TMLogger.error('File does not exist: ' + filePath);
                    return false;
                }
                var result = this.moveFileToQueue(taskId, filePath);
                if (!result) {
                    return false;
                }
            }
            var result = this.createTaskTicket(taskId, params);
            if (!result) {
                return false;
            }
            TMLogger.info("Queueing task: " + taskId);
            TMHelper.call('task/queue', {
                'id': taskId
            });
            return true;
        } catch (myException) {
            TMLogger.exception("TMTaskQueue.addTask", myException + ' (tm_task_queue:64)');
            return false;
        }
    },
    moveFileToQueue: function (taskId, filePath) {
        try {
            this.createFolderIfNeeded();
            TMLogger.info('Adding task ' + taskId + ' for file: ' + filePath);
            var dstFile = this.getTaskFolderPath() + '/' + taskId;
            TMLogger.info('Moving file to: ' + dstFile);
            TMFiles.move(filePath, dstFile);
            return true;
        } catch (myException) {
            TMLogger.exception("TMTaskQueue.addTask", myException + ' (tm_task_queue:78)');
            return false;
        }
    },
    createTaskTicket: function (taskId, params) {
        try {
            this.createFolderIfNeeded();
            params['twixl_version'] = TMVersion.version;
            TMLogger.info('Adding task ticket ' + taskId);
            TMLogger.info('Task ticket path: ' + this.getTaskFolderPath() + '/' + taskId + '.task');
            var filePath = this.getTaskFolderPath() + '/' + taskId + '.task';
            var fileData = TMJSON.stringify(params);
            TMFiles.writeUTF8DataToFile(filePath, fileData);
            return true;
        } catch (myException) {
            TMLogger.exception("TMTaskQueue.addTask", myException + ' (tm_task_queue:100)');
            return false;
        }
    },
    getStatus: function () {
        try {
            var status = TMHelper.call('status/queue');
            return TMJSON.stringify(status);
        } catch (myException) {
            TMLogger.exception("TMTaskQueue.getStatus", myException + ' (tm_task_queue:110)');
            return TMJSON.stringify({
                'status': '',
                'progress': 0,
                'viewers': []
            });
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMTimer = {
    times: {},
    start: function (id, message) {
        try {
            if (message) {
                TMLogger.info(message);
                this.times[id] = this.getTime();
            } else {
                this.times[id] = this.getMicroTime();
            }
        } catch (myException) {
            TMLogger.exception("TMTimer.start", myException + ' (tm_timer:19)');
        }
    },
    printElapsed: function (id, myLabel) {
        try {
            if (myLabel) {
                var myElapsed = Math.round(this.getTime() - this.times[id]);
                TMLogger.info(myLabel + ': ' + this.formatElapsed(myElapsed));
            } else {
                var myElapsed = this.getMicroTime() - this.times[id];
                TMLogger.info('        [' + id + '] ' + myElapsed + ' ms');
            }
        } catch (myException) {
            TMLogger.exception("TMTimer.printElpased", myException + ' (tm_timer:33)');
        }
    },
    getTime: function () {
        try {
            var myDate = new Date();
            return (myDate.getTime() / 1000);
        } catch (myException) {
            TMLogger.exception("TMTimer.getTime", myException + ' (tm_timer:42)');
        }
    },
    getMicroTime: function () {
        try {
            var myDate = new Date();
            return myDate.getTime();
        } catch (myException) {
            TMLogger.exception("TMTimer.getMicroTime", myException + ' (tm_timer:51)');
        }
    },
    formatElapsed: function (elapsed) {
        try {
            var hours = Math.floor(elapsed / 3600);
            var minutes = Math.floor((elapsed - (hours * 3600)) / 60);
            var seconds = elapsed - (hours * 3600) - (minutes * 60);
            if (hours < 10) {
                hours = "0" + hours;
            }
            if (minutes < 10) {
                minutes = "0" + minutes;
            }
            if (seconds < 10) {
                seconds = "0" + seconds;
            }
            return hours + ':' + minutes + ':' + seconds;
        } catch (myException) {
            TMLogger.exception("TMTimer.formatElapsed", myException + ' (tm_timer:69)');
            return elapsed + ' s';
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMUI = {
    UI_LABEL_WIDTH: 90,
    UI_ITEM_WIDTH: 340,
    setFontSize: function (label, size) {
        try {
            label.graphics.font = "dialog:" + size;
        } catch (myException) {
            TMLogger.exception("TMUI.setFontSize", myException + ' (tm_ui:15)');
        }
    },
    setFontColor: function (label, window, color) {
        try {
            label.graphics.foregroundColor = label.graphics.newPen(window.graphics.PenType.SOLID_COLOR, color, 1);
        } catch (myException) {
            TMLogger.exception("TMUI.setBoldFontSize", myException + ' (tm_ui:23)');
        }
    },
    alert: function (msg, title) {
        try {
            var myDialog = new Window('dialog', '', undefined, {
                closeButton: false
            });
            myDialog.orientation = 'column';
            myDialog.margins = [20, 20, 20, 20];
            var myAlert = myDialog.add('statictext', undefined, '', {
                multiline: true
            });
            myAlert.minimumSize.width = 380;
            myAlert.text = (title) ? title : 'Alert';
            var font = myAlert.graphics.font;
            myAlert.graphics.font = font.name + '-Bold:' + font.size;
            var myMessage = myDialog.add('statictext', undefined, '', {
                multiline: true
            });
            myMessage.minimumSize.width = 380;
            myMessage.minimumSize.height = 60;
            myMessage.text = msg;
            var myButtonGroup = myDialog.add('group');
            myButtonGroup.alignment = "right";
            var myOkButton = myButtonGroup.add("button", undefined, "OK");
            myDialog.defaultElement = myOkButton;
            return myDialog.show();
        } catch (myException) {
            TMLogger.exception("TMUI.alert", myException + ' (tm_ui:56)');
        }
    },
    confirm: function (msg, title) {
        try {
            var myDialog = new Window('dialog', '', undefined, {
                closeButton: false
            });
            myDialog.orientation = 'column';
            myDialog.margins = [20, 20, 20, 20];
            var myAlert = myDialog.add('statictext', undefined, '', {
                multiline: true
            });
            myAlert.minimumSize.width = 380;
            myAlert.text = (title) ? title : 'Are you sure?';
            var font = myAlert.graphics.font;
            myAlert.graphics.font = font.name + '-Bold:' + font.size;
            var myMessage = myDialog.add('statictext', undefined, '', {
                multiline: true
            });
            myMessage.minimumSize.width = 380;
            myMessage.minimumSize.height = 60;
            myMessage.text = msg;
            var myButtonGroup = myDialog.add('group');
            myButtonGroup.alignment = "right";
            var myNoButton = myButtonGroup.add("button", undefined, "No");
            var myYesButton = myButtonGroup.add("button", undefined, "Yes");
            myDialog.cancelElement = myNoButton;
            myDialog.defaultElement = myYesButton;
            return myDialog.show();
        } catch (myException) {
            TMLogger.exception("TMUI.confirm", myException + ' (tm_ui:91)');
        }
    },
    fixLabelWidth: function (label) {
        try {
            label.minimumSize.width = this.UI_LABEL_WIDTH;
            label.justify = "right";
        } catch (myException) {
            TMLogger.exception("TMUI.fixLabelWidth", myException + ' (tm_ui:100)');
        }
    },
    fixDropDownListWidth: function (dropDownList) {
        try {
            dropDownList.minimumSize.width = this.UI_ITEM_WIDTH;
        } catch (myException) {
            TMLogger.exception("TMUI.fixDropDownListWidth", myException + ' (tm_ui:108)');
        }
    },
    fixEditBoxWidth: function (editBox, width) {
        try {
            if (!width) {
                width = this.UI_ITEM_WIDTH;
            }
            if (editBox.properties && editBox.properties['hasButton']) {
                editBox.minimumSize.width = width - 40;
            } else {
                editBox.minimumSize.width = width;
            }
        } catch (myException) {
            TMLogger.exception("TMUI.fixEditBoxWidth", myException + ' (tm_ui:123)');
        }
    },
    fixLabelWidthsForDialog: function (myDialog) {
        try {
            this.fixLabelWidthsForChildren(myDialog.children);
        } catch (myException) {
            TMLogger.exception("TMUI.fixLabelWidthsForDialog", myException + ' (tm_ui:131)');
        }
    },
    fixLabelWidthsForChildren: function (myChildren) {
        try {
            if (myChildren.length == 0) {
                return;
            }
            for (var i = 0; i < myChildren.length; i++) {
                var myChild = myChildren[i];
                if (myChild.constructor.name == 'StaticText') {
                    this.fixLabelWidth(myChild);
                }
                if (myChild.constructor.name == 'DropDownList') {
                    this.fixDropDownListWidth(myChild);
                }
                if (myChild.constructor.name == 'EditText') {
                    this.fixEditBoxWidth(myChild);
                }
                if (myChild.constructor.name == 'Button' && myChild.text == '...') {
                    myChild.minimumSize.width = 30;
                    myChild.preferredSize.width = 30;
                    myChild.maximumSize.width = 30;
                }
                if (myChild && myChild.children) {
                    TMUI.fixLabelWidthsForChildren(myChild.children);
                }
            };
        } catch (myException) {
            TMLogger.exception("TMUI.fixLabelWidthsForChildren", myException + ' (tm_ui:161)');
        }
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
if (kMaxRecursive301Calls == undefined) {
    const kMaxRecursive301Calls = 10;
}
var TMURL = {
    get: function (url, isBinary, recursive301CallLevel, timeout) {
        try {
            var reply = null;
            const kUTF8CharState_Complete = 0;
            const kUTF8CharState_PendingMultiByte = 1;
            const kUTF8CharState_Binary = 2;
            const kLineState_InProgress = 0;
            const kLineState_SeenCR = 1;
            const kProtocolState_Status = 1;
            const kProtocolState_Headers = 2;
            const kProtocolState_Body = 3;
            const kProtocolState_Complete = 4;
            const kProtocolState_TimeOut = 5;
            do {
                var parsedURL = this.parseURL(url);
                if (parsedURL.protocol != "HTTP") {
                    break;
                }
                if (isBinary) {
                    var request = "GET /" + parsedURL.path + " HTTP/1.0\r\n" +
                        "Host: " + parsedURL.address + "\r\n" +
                        "User-Agent: InDesign ExtendScript\r\n" +
                        "Accept: */*\r\n" +
                        "Connection: keep-alive\r\n\r\n";
                } else {
                    var request = "GET /" + parsedURL.path + " HTTP/1.0\r\n" +
                        "Host: " + parsedURL.address + ":" + parsedURL.port + "\r\n" +
                        "User-Agent: InDesign ExtendScript\r\n" +
                        "Accept: */*\r\n" +
                        "Accept-Encoding:\r\n" +
                        "Connection: keep-alive\r\n" +
                        "Accept-Language: *\r\n" +
                        "Accept-Charset: utf-8\r\n\r\n";
                }
                var socket = new Socket();
                if (!socket.open(parsedURL.address + ":" + parsedURL.port, "BINARY")) {
                    break;
                }
                socket.write(request);
                var readState = {
                    buffer: "",
                    bufPos: 0,
                    curCharState: isBinary ? kUTF8CharState_Binary : kUTF8CharState_Complete,
                    curCharCode: 0,
                    pendingUTF8Bytes: 0,
                    lineState: kLineState_InProgress,
                    curLine: "",
                    line: "",
                    isLineReadyToProcess: false,
                    protocolState: kProtocolState_Status,
                    contentBytesPending: null,
                    dataAvailable: true,
                    status: "",
                    headers: {},
                    body: ""
                };
                while (
                    !(readState.protocolState == kProtocolState_Complete && readState.buffer.length <= readState.bufPos) &&
                    readState.protocolState != kProtocolState_TimeOut
                ) {
                    if (readState.bufPos > 0 && readState.buffer.length == readState.bufPos) {
                        readState.buffer = "";
                        readState.bufPos = 0;
                    }
                    if (readState.buffer == "") {
                        if (readState.protocolState == kProtocolState_Body) {
                            if (readState.contentBytesPending == null) {
                                if (!readState.dataAvailable && !socket.connected) {
                                    socket = null;
                                    readState.protocolState = kProtocolState_Complete;
                                } else {
                                    readState.buffer += socket.read();
                                    readState.dataAvailable = readState.buffer.length > 0;
                                    if (!readState.dataAvailable) {
                                        readState.buffer += socket.read(1);
                                        readState.dataAvailable = readState.buffer.length > 0;
                                    }
                                }
                            } else {
                                if (!readState.dataAvailable && !socket.connected) {
                                    socket = null;
                                    readState.protocolState = kProtocolState_TimeOut;
                                } else {
                                    readState.buffer = socket.read(readState.contentBytesPending);
                                    readState.dataAvailable = readState.buffer.length > 0;
                                    readState.contentBytesPending -= readState.buffer.length;
                                    if (readState.contentBytesPending == 0) {
                                        readState.protocolState = kProtocolState_Complete;
                                        socket.close();
                                        socket = null;
                                    }
                                    if (isBinary) {
                                        readState.body += readState.buffer;
                                        readState.buffer = "";
                                        readState.bufPos = 0;
                                    }
                                }
                            }
                        } else if (readState.protocolState != kProtocolState_Complete) {
                            if (!readState.dataAvailable && !socket.connected) {
                                socket = null;
                                readState.protocolState = kProtocolState_TimeOut;
                            } else {
                                readState.buffer += socket.read(1);
                                readState.dataAvailable = readState.buffer.length > 0;
                            }
                        }
                    }
                    if (readState.buffer.length > readState.bufPos) {
                        if (readState.curCharState == kUTF8CharState_Binary && readState.protocolState == kProtocolState_Body) {
                            readState.body += readState.buffer;
                            readState.bufPos = readState.buffer.length;
                        } else {
                            var cCode = readState.buffer.charCodeAt(readState.bufPos++);
                            switch (readState.curCharState) {
                                case kUTF8CharState_Binary:
                                    readState.curCharCode = cCode;
                                    readState.multiByteRemaining = 0;
                                    break;
                                case kUTF8CharState_Complete:
                                    if (cCode <= 127) {
                                        readState.curCharCode = cCode;
                                        readState.multiByteRemaining = 0;
                                    } else if ((cCode & 0xE0) == 0xC0) {
                                        readState.curCharCode = cCode & 0x1F;
                                        readState.curCharState = kUTF8CharState_PendingMultiByte;
                                        readState.pendingUTF8Bytes = 1;
                                    } else if ((cCode & 0xF0) == 0xE0) {
                                        readState.curCharCode = cCode & 0x0F;
                                        readState.curCharState = kUTF8CharState_PendingMultiByte;
                                        readState.pendingUTF8Bytes = 2;
                                    } else if ((cCode & 0xF8) == 0xF0) {
                                        readState.curCharCode = cCode & 0x07;
                                        readState.curCharState = kUTF8CharState_PendingMultiByte;
                                        readState.pendingUTF8Bytes = 3;
                                    } else {
                                        readState.curCharCode = cCode;
                                        readState.pendingUTF8Bytes = 0;
                                    }
                                    break;
                                case kUTF8CharState_PendingMultiByte:
                                    if ((cCode & 0xC0) == 0x80) {
                                        readState.curCharCode = (readState.curCharCode << 6) | (cCode & 0x3F);
                                        readState.pendingUTF8Bytes--;
                                        if (readState.pendingUTF8Bytes == 0) {
                                            readState.curCharState = kUTF8CharState_Complete;
                                        }
                                    } else {
                                        readState.curCharCode = cCode;
                                        readState.multiByteRemaining = 0;
                                        readState.curCharState = kUTF8CharState_Complete;
                                    }
                                    break;
                            }
                            if (readState.curCharState == kUTF8CharState_Complete || readState.curCharState == kUTF8CharState_Binary) {
                                cCode = readState.curCharCode;
                                var c = String.fromCharCode(readState.curCharCode);
                                if (readState.protocolState == kProtocolState_Body || readState.protocolState == kProtocolState_Complete) {
                                    readState.body += c;
                                } else {
                                    if (readState.lineState == kLineState_SeenCR) {
                                        readState.line = readState.curLine;
                                        readState.isLineReadyToProcess = true;
                                        readState.curLine = "";
                                        readState.lineState = kLineState_InProgress;
                                        if (cCode == 13) {
                                            readState.lineState = kLineState_SeenCR;
                                        } else if (cCode != 10) {
                                            readState.curLine += c;
                                        }
                                    } else if (readState.lineState == kLineState_InProgress) {
                                        if (cCode == 13) {
                                            readState.lineState = kLineState_SeenCR;
                                        } else if (cCode == 10) {
                                            readState.line = readState.curLine;
                                            readState.isLineReadyToProcess = true;
                                            readState.curLine = "";
                                        } else {
                                            readState.curLine += c;
                                        }
                                    }
                                    if (readState.isLineReadyToProcess) {
                                        readState.isLineReadyToProcess = false;
                                        if (readState.protocolState == kProtocolState_Status) {
                                            readState.status = readState.line;
                                            readState.protocolState = kProtocolState_Headers;
                                        } else if (readState.protocolState == kProtocolState_Headers) {
                                            if (readState.line == "") {
                                                readState.protocolState = kProtocolState_Body;
                                            } else {
                                                var headerLine = readState.line.split(":");
                                                var headerTag = headerLine[0].replace(/^\s*(.*\S)\s*$/, "$1");
                                                headerLine = headerLine.slice(1).join(":");
                                                headerLine = headerLine.replace(/^\s*(.*\S)\s*$/, "$1");
                                                readState.headers[headerTag] = headerLine;
                                                if (headerTag == "Content-Length") {
                                                    readState.contentBytesPending = parseInt(headerLine);
                                                    if (isNaN(readState.contentBytesPending) || readState.contentBytesPending <= 0) {
                                                        readState.contentBytesPending = null;
                                                    } else {
                                                        readState.contentBytesPending -= (readState.buffer.length - readState.bufPos);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (socket != null) {
                    socket.close();
                    socket = null;
                }
                if (readState.body) {
                    readState.body = readState.body.replace(/^\s+|\s+$/g, '');
                }
                var reply = {
                    status: readState.status,
                    headers: readState.headers,
                    body: readState.body
                };
            } while (false);
            if (reply != undefined && reply.status.indexOf("301") >= 0) {
                if (recursive301CallLevel == undefined) {
                    recursive301CallLevel = 0;
                }
                if (recursive301CallLevel < kMaxRecursive301Calls) {
                    reply = GetURL(reply.headers.Location, isBinary, recursive301CallLevel + 1);
                }
            }
            if (reply != undefined) {
                return reply.body;
            } else {
                return '';
            }
        } catch (myException) {
            TMStackTrace.addToStack('TMURL.get', myException);
        }
    },
    parseURL: function (url) {
        try {
            url = url.replace(/([a-z]*):\/\/([-\._a-z0-9A-Z]*)(:[0-9]*)?\/?(.*)/, "$1/$2/$3/$4");
            url = url.split("/");
            if (url[2] == "undefined") {
                url[2] = "80";
            }
            var parsedURL = {
                protocol: url[0].toUpperCase(),
                address: url[1],
                port: url[2],
                path: ""
            };
            url = url.slice(3);
            parsedURL.path = url.join("/");
            if (parsedURL.port.charAt(0) == ':') {
                parsedURL.port = parsedURL.port.slice(1);
            }
            if (parsedURL.port != "") {
                parsedURL.port = parseInt(parsedURL.port);
            }
            if (parsedURL.port == "" || parsedURL.port < 0 || parsedURL.port > 65535) {
                parsedURL.port = 80;
            }
            parsedURL.path = parsedURL.path;
            return parsedURL;
        } catch (myException) {
            TMStackTrace.addToStack('TMURL.parseURL', myException);
        }
    },
}
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
String.prototype.tmStartsWith = function (str) {
    return this.slice(0, str.length) == str;
};
String.prototype.tmEndsWith = function (str) {
    return this.slice(-str.length) == str;
};
String.prototype.tmContains = function (str) {
    return this.indexOf(str) != -1;
};
String.prototype.tmRepeat = function (num) {
    return new Array(num + 1).join(this);
};
String.prototype.tmTrim = function () {
    return this.replace(/^\s+|\s+$/g, '');
};
String.prototype.tmFileExtension = function () {
    var myFileExt = '';
    var myFileNameParts = this.split('.');
    if (myFileNameParts.length > 1) {
        myFileExt = myFileNameParts[myFileNameParts.length - 1];
    }
    return myFileExt.toLowerCase();
};
var TMDeviceUtilities = {
    friendlyDeviceType: function (deviceType) {
        if (deviceType == 'ipad') {
            deviceType = 'iPad';
        } else if (deviceType == 'ipad-retina' || deviceType == 'ipad_retina') {
            deviceType = 'iPad Retina';
        } else if (deviceType == 'android10') {
            deviceType = 'Android 10 inch';
        } else if (deviceType == 'android7') {
            deviceType = 'Android 7 inch';
        } else if (deviceType == 'android') {
            deviceType = 'Android';
        } else if (deviceType == 'phone_s' || deviceType == 'phone-s') {
            deviceType = 'Phone Small';
        } else if (deviceType == 'phone_m' || deviceType == 'phone-m') {
            deviceType = 'Phone Medium';
        } else if (deviceType == 'phone_l' || deviceType == 'phone-l') {
            deviceType = 'Phone Large';
        }
        return deviceType;
    },
};
var TMUtilities = {
    decodeURI: function (str) {
        try {
            return decodeURI(str);
        } catch (myException) {
            return str;
        }
    },
    dataFolderPath: function () {
        return Folder.appData.fsName + "\\Twixl Publisher";
    },
    dispatchEvent: function (type, data) {
        if (TMPluginCore.engine != 'html') {
            return;
        }
        data = TMJSON.stringify(data);
        TMLogger.debug('Dispatching event: ' + type + ' ' + data);
        var eventObj = new CSXSEvent();
        eventObj.type = type;
        eventObj.data = data;
        eventObj.dispatch();
    },
    stringWithDefault: function (obj, key, defaultValue) {
        if (obj.hasOwnProperty(key) && obj[key] != undefined) {
            return obj[key];
        }
        return defaultValue;
    },
    bool: function (value) {
        if (value === 'yes' || value === 'y' || value === '1' || value === 1 || value === true || value === 'true') {
            return true;
        } else {
            return false;
        }
    },
    boolWithDefault: function (obj, key, defaultValue) {
        if (obj.hasOwnProperty(key) && obj[key] != undefined) {
            var value = obj[key];
            if (value === 'yes' || value === 'y' || value === '1' || value === 1 || value === true) {
                return true;
            } else {
                return false;
            }
        }
        return defaultValue;
    },
    urlSerialize: function (obj) {
        var str = [];
        for (var p in obj) {
            str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
        }
        var result = str.join("&");
        if (result.length > 0) {
            result = '?' + result;
        }
        return result;
    },
    addLeadingZeroToNumber: function (num, size) {
        var s = num + "";
        while (s.length < size) {
            s = "0" + s
        };
        return s;
    },
    getProperty: function (object, propertyName, propertyDefault) {
        try {
            if (object == undefined) {
                return propertyDefault;
            }
            var value = object.hasOwnProperty(propertyName);
            if (value != undefined && value != "") {
                return object[propertyName];
            }
        } catch (myException) {
            TMLogger.exception('TMUtilities.getProperty', myException + ' (tm_utilities:143)');
        }
        return propertyDefault;
    },
    getColorProperty: function (object, propertyName, propertyDefault) {
        try {
            if (object == undefined) {
                return propertyDefault;
            }
            if (object.hasOwnProperty(propertyName)) {
                var myColor = object[propertyName].toString(16).toUpperCase();
                while (myColor.length < 6) {
                    myColor = "0" + myColor;
                }
                return myColor;
            }
        } catch (myException) {
            TMLogger.exception('TMUtilities.getColorProperty', myException + ' (tm_utilities:161)');
        }
        return propertyDefault;
    },
    getBoolProperty: function (object, propertyName, propertyDefault) {
        try {
            if (object == undefined) {
                return propertyDefault;
            }
            if (object.hasOwnProperty(propertyName)) {
                var myValue = object[propertyName];
                if (myValue == true || myValue == "yes") {
                    return true;
                } else {
                    return false;
                }
            }
        } catch (myException) {
            TMLogger.exception('TMUtilities.getBoolProperty', myException + ' (tm_utilities:180)');
        }
        return propertyDefault;
    },
    formatFileSize: function (bytes, precision) {
        try {
            if (precision == undefined) {
                precision = 2;
            }
            var units = {
                '1000000000000000000': 'EB',
                '1000000000000000': 'PB',
                '1000000000000': 'TB',
                '1000000000': 'GB',
                '1000000': 'MB',
                '1000': 'KB'
            };
            if (bytes < 1000) {
                return bytes + ' bytes';
            }
            for (var myBase in units) {
                var myTitle = units[myBase];
                if (Math.floor(bytes / myBase) != 0) {
                    return (bytes / myBase).toFixed(precision) + ' ' + myTitle;
                }
            }
        } catch (myException) {
            TMLogger.exception("TMUtilities.formatFileSize", myException + ' (tm_utilities:214)');
            return bytes + ' bytes';
        }
    },
    mergeArrays: function (myArray1, myArray2) {
        try {
            for (var i in myArray2) {
                myArray1[i] = myArray2[i];
            }
            return myArray1;
        } catch (myException) {
            TMStackTrace.addToStack("TMUtilities.mergeArrays", myException);
        }
    },
    normalizeUrl: function (url) {
        try {
            url = url.tmTrim();
            if (url.tmEndsWith('$$$/Dialog/Behaviors/GoToView/DefaultURL')) {
                return undefined;
            }
            var baseUrl = decodeURIComponent(url);
            if (baseUrl.tmStartsWith('mailto:') || baseUrl.tmStartsWith('tel:') || baseUrl.tmStartsWith('callto:')) {
                return url;
            }
            if (baseUrl.indexOf('?') > -1) {
                baseUrl = baseUrl.substring(0, baseUrl.indexOf('?'));
            }
            if (baseUrl.indexOf('#') > -1) {
                baseUrl = baseUrl.substring(0, baseUrl.indexOf('#'));
            }
            if (baseUrl.indexOf('://') === -1) {
                url = 'http://' + url;
            }
            url = TMFiles.slugURL(url);
            return url;
        } catch (myException) {
            TMLogger.exception("TMUtilities.normalizeUrl", myException + ' (tm_utilities:255)');
            return url;
        }
    },
    uniqueArrayValues: function (myArray) {
        try {
            var o = {};
            var l = myArray.length;
            var r = [];
            for (var i = 0; i < l; i++) {
                o[myArray[i]] = myArray[i];
            }
            for (i in o) {
                r.push(o[i]);
            }
            return r;
        } catch (myException) {
            TMLogger.exception("TMUtilities.uniqueArrayValues", myException + ' (tm_utilities:273)');
            return myArray;
        }
    },
    objectKeys: function (myObject) {
        try {
            var keys = [];
            for (var key in myObject) {
                keys.push(key);
            }
            return keys;
        } catch (myException) {
            TMLogger.exception("TMUtilities.objectKeys", myException + ' (tm_utilities:286)');
            return [];
        }
    },
    objectIsEmpty: function (myObject) {
        try {
            for (var key in myObject) {
                return false;
            }
            return true;
        } catch (myException) {
            TMLogger.silentException("TMUtilities.objectIsEmpty", myException);
            return true;
        }
    },
    upCast: function (pageItem) {
        try {
            var retVal = pageItem;
            try {
                retVal = pageItem.getElements()[0];
            } catch (myException) {
                retVal = pageItem;
            }
            return retVal;
        } catch (myException) {
            TMLogger.silentException("TMUtilities.upCast", myException);
            return pageItem;
        }
    },
    collectionToArray: function (theCollection) {
        try {
            return theCollection.everyItem().getElements().slice(0);
        } catch (myException) {
            TMLogger.silentException("TMUtilities.collectionToArray", myException);
            return [];
        }
    },
    itemInArray: function (myArray, myItem) {
        try {
            if (!myArray || !myItem) {
                return false;
            }
            var myItemLower = myItem;
            try {
                myItemLower = myItemLower.toLowerCase();
            } catch (e) { }
            for (i = 0; i < myArray.length; i++) {
                var myItem2Lower = myArray[i];
                try {
                    myItem2Lower = myItem2Lower.toLowerCase();
                } catch (e) { }
                if (myItem2Lower === myItemLower) {
                    return true;
                }
            }
            return false;
        } catch (myException) {
            TMStackTrace.addToStack('TMUtilities.itemInArray', myException);
        }
    },
    removeArrayItemByIndex: function (myArray, index) {
        var result = [];
        for (var i = 0; i < myArray.length; i++) {
            if (i != index) {
                result.push(myArray[i]);
            }
        }
        return result;
    },
    keyCount: function (myObject) {
        try {
            var myCount = 0;
            for (var myProp in myObject) {
                myCount++;
            }
            return myCount;
        } catch (myException) {
            TMLogger.exception("TMUtilities.keyCount", myException + ' (tm_utilities:375)');
            return 0;
        }
    },
    cloneObject: function (myObject) {
        try {
            var myClone = {};
            for (var i in myObject) {
                var myProp = myObject[i];
                if (myProp && typeof myProp == 'object') {
                    myClone[i] = this.cloneObject(myProp);
                } else {
                    myClone[i] = myProp;
                }
            }
            return myClone;
        } catch (myException) {
            TMLogger.exception("TMUtilities.cloneObject", myException + ' (tm_utilities:393)');
            return undefined;
        }
    },
    uuid: function () {
        try {
            var d = new Date().getTime();
            var uuid = 'xxxxxxxxxxxxxxxxyxxxxxxxxxxxxxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = (d + Math.random() * 16) % 16 | 0;
                d = Math.floor(d / 16);
                return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
            });
            return uuid.toLowerCase();
        } catch (myException) {
            TMLogger.exception("TMUtilities.uuid", myException + ' (tm_utilities:408)');
            return undefined;
        }
    },
    hexColor: function (myValue) {
        try {
            if (myValue == undefined) {
                return "";
            }
            var length = 6 - myValue.length;
            for (var i = 0; i < length; i++) {
                myValue = '0' + myValue;
            }
            return myValue.toUpperCase();
        } catch (myException) {
            TMLogger.exception("TMUtilities.hexColor", myException + ' (tm_utilities:424)');
            return undefined;
        }
    },
    isValidCssColor: function (color_string) {
        try {
            color_string = color_string.toString();
            if (color_string.length != 6) {
                TMLogger.info('not correct length');
                return false;
            }
            var bits = /^(\w{2})(\w{2})(\w{2})$/.exec(color_string);
            if (bits) {
                var r = parseInt(bits[1], 16);
                var g = parseInt(bits[2], 16);
                var b = parseInt(bits[3], 16);
                return true;
            }
            return false;
        } catch (myException) {
            TMLogger.exception("TMUtilities.hexColor", myException + ' (tm_utilities:454)');
            return false;
        }
    },
};
var TMVersionUtilities = {
    getMajorVersion: function (version) {
        try {
            var versionParts = version.split('.');
            var majorVersion = '';
            if (versionParts.length == 1) {
                majorVersion = TMUtilities.addLeadingZeroToNumber(versionParts[0], 2) + '00';
            } else if (versionParts.length >= 2) {
                majorVersion = TMUtilities.addLeadingZeroToNumber(versionParts[0], 2) + TMUtilities.addLeadingZeroToNumber(versionParts[1], 2);
            }
            return Number(majorVersion);
        } catch (myException) {
            TMStackTrace.addToStack("TMVersionUtilities.getMajorVersion", myException);
        }
    },
};
var TMVersion = {
    version: '9.3',
    build: '32750',
    codename: 'Highland Park',
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMWidget = {
    encodeParameter: function (value) {
        if (value === true || value === 1) {
            return "1";
        }
        if (value === false || value === 0) {
            return "0";
        }
        return encodeURIComponent(value).replace(/ /g, "+");
    },
};
/**
 *  (c) Copyright Twixl media, http://twixlmedia.com
 *  Created by Pieter Claerhout, pieter@twixlmedia.com
 */
var TMXml = {
    startTag: function (myXml, myTag, myProps) {
        try {
            myXml.write('<' + myTag);
            if (myProps != undefined) {
                for (var myPropName in myProps) {
                    if (myProps[myPropName] != undefined) {
                        var myProp = this.getXmlAttribute(myProps[myPropName]);
                        myXml.write(' ' + myPropName + '="' + myProp + '"');
                    }
                }
            }
            myXml.write('>');
        } catch (myException) {
            TMLogger.exception("TMXml.startTag (" + myTag + ")", myException + ' (tm_xml:21)');
        }
    },
    closeTag: function (myXml, myTag) {
        try {
            myXml.write('</' + myTag + '>');
        } catch (myException) {
            TMLogger.exception("TMXml.closeTag (" + myTag + ")", myException + ' (tm_xml:29)');
        }
    },
    writeTag: function (myXml, myTag, myProps, myClose) {
        try {
            myXml.write('<' + myTag);
            if (myProps != undefined) {
                for (var myPropName in myProps) {
                    var myValue = myProps[myPropName];
                    if (myValue instanceof Array || myValue == undefined) {
                        continue;
                    }
                    var myProp = this.getXmlAttribute(myValue);
                    myXml.write(' ' + myPropName + '="' + myProp + '"');
                }
            }
            if (myClose == true) {
                myXml.write('/>');
            } else {
                myXml.write('>');
            }
        } catch (myException) {
            TMLogger.exception("TMXml.writeTag (" + myTag + ")", myException + ' (tm_xml:52)');
        }
    },
    writeTextTag: function (myXml, myTag, myValue) {
        try {
            myXml.write('<' + myTag + '>' + this.getXmlValue(myValue) + '</' + myTag + '>');
        } catch (myException) {
            TMLogger.exception("TMXml.writeTextTag (" + myTag + ")", myException + ' (tm_xml:60)');
        }
    },
    writeArray: function (myXml, myList, myCollectionName, myItemName) {
        try {
            if (myList == undefined) {
                return;
            }
            if (myList.length > 0) {
                TMXml.startTag(myXml, myCollectionName);
                var myListLength = myList.length;
                for (var i = 0; i < myListLength; i++) {
                    TMXml.writeTag(myXml, myItemName, myList[i], true);
                }
                TMXml.closeTag(myXml, myCollectionName);
            }
        } catch (myException) {
            TMLogger.exception("TMXml.writeArray(" + myCollectionName + ")", myException + ' (tm_xml:78)');
        }
    },
    getXmlValue: function (string) {
        try {
            if (string == undefined) {
                return "";
            }
            var xml = new XML("<xml></xml>");
            xml.appendChild(string);
            var xmlString = xml.toXMLString();
            var xmlValue = xmlString.substr(5, xmlString.length - 11);
            return this.stripNonPrintables(xmlValue);
        } catch (myException) {
            TMStackTrace.addToStack("TMXml.getXmlValue", myException);
        }
    },
    getXmlAttribute: function (string) {
        try {
            if (string === true) {
                return "yes";
            } else if (string === false) {
                return "no";
            } else {
                var xml = new XML("<xml></xml>");
                xml.@att = string;
                var xmlString = xml.toXMLString();
                xmlValue = xmlString.substr(10, xmlString.length - 13);
                return this.stripNonPrintables(xmlValue);
            }
        } catch (myException) {
            TMStackTrace.addToStack("TMXml.getXmlAttribute", myException);
        }
    },
    stripNonPrintables: function (string) {
        var re = /[\0-\x1F\x7F-\x9F\xAD\u0378\u0379\u037F-\u0383\u038B\u038D\u03A2\u0528-\u0530\u0557\u0558\u0560\u0588\u058B-\u058E\u0590\u05C8-\u05CF\u05EB-\u05EF\u05F5-\u0605\u061C\u061D\u06DD\u070E\u070F\u074B\u074C\u07B2-\u07BF\u07FB-\u07FF\u082E\u082F\u083F\u085C\u085D\u085F-\u089F\u08A1\u08AD-\u08E3\u08FF\u0978\u0980\u0984\u098D\u098E\u0991\u0992\u09A9\u09B1\u09B3-\u09B5\u09BA\u09BB\u09C5\u09C6\u09C9\u09CA\u09CF-\u09D6\u09D8-\u09DB\u09DE\u09E4\u09E5\u09FC-\u0A00\u0A04\u0A0B-\u0A0E\u0A11\u0A12\u0A29\u0A31\u0A34\u0A37\u0A3A\u0A3B\u0A3D\u0A43-\u0A46\u0A49\u0A4A\u0A4E-\u0A50\u0A52-\u0A58\u0A5D\u0A5F-\u0A65\u0A76-\u0A80\u0A84\u0A8E\u0A92\u0AA9\u0AB1\u0AB4\u0ABA\u0ABB\u0AC6\u0ACA\u0ACE\u0ACF\u0AD1-\u0ADF\u0AE4\u0AE5\u0AF2-\u0B00\u0B04\u0B0D\u0B0E\u0B11\u0B12\u0B29\u0B31\u0B34\u0B3A\u0B3B\u0B45\u0B46\u0B49\u0B4A\u0B4E-\u0B55\u0B58-\u0B5B\u0B5E\u0B64\u0B65\u0B78-\u0B81\u0B84\u0B8B-\u0B8D\u0B91\u0B96-\u0B98\u0B9B\u0B9D\u0BA0-\u0BA2\u0BA5-\u0BA7\u0BAB-\u0BAD\u0BBA-\u0BBD\u0BC3-\u0BC5\u0BC9\u0BCE\u0BCF\u0BD1-\u0BD6\u0BD8-\u0BE5\u0BFB-\u0C00\u0C04\u0C0D\u0C11\u0C29\u0C34\u0C3A-\u0C3C\u0C45\u0C49\u0C4E-\u0C54\u0C57\u0C5A-\u0C5F\u0C64\u0C65\u0C70-\u0C77\u0C80\u0C81\u0C84\u0C8D\u0C91\u0CA9\u0CB4\u0CBA\u0CBB\u0CC5\u0CC9\u0CCE-\u0CD4\u0CD7-\u0CDD\u0CDF\u0CE4\u0CE5\u0CF0\u0CF3-\u0D01\u0D04\u0D0D\u0D11\u0D3B\u0D3C\u0D45\u0D49\u0D4F-\u0D56\u0D58-\u0D5F\u0D64\u0D65\u0D76-\u0D78\u0D80\u0D81\u0D84\u0D97-\u0D99\u0DB2\u0DBC\u0DBE\u0DBF\u0DC7-\u0DC9\u0DCB-\u0DCE\u0DD5\u0DD7\u0DE0-\u0DF1\u0DF5-\u0E00\u0E3B-\u0E3E\u0E5C-\u0E80\u0E83\u0E85\u0E86\u0E89\u0E8B\u0E8C\u0E8E-\u0E93\u0E98\u0EA0\u0EA4\u0EA6\u0EA8\u0EA9\u0EAC\u0EBA\u0EBE\u0EBF\u0EC5\u0EC7\u0ECE\u0ECF\u0EDA\u0EDB\u0EE0-\u0EFF\u0F48\u0F6D-\u0F70\u0F98\u0FBD\u0FCD\u0FDB-\u0FFF\u10C6\u10C8-\u10CC\u10CE\u10CF\u1249\u124E\u124F\u1257\u1259\u125E\u125F\u1289\u128E\u128F\u12B1\u12B6\u12B7\u12BF\u12C1\u12C6\u12C7\u12D7\u1311\u1316\u1317\u135B\u135C\u137D-\u137F\u139A-\u139F\u13F5-\u13FF\u169D-\u169F\u16F1-\u16FF\u170D\u1715-\u171F\u1737-\u173F\u1754-\u175F\u176D\u1771\u1774-\u177F\u17DE\u17DF\u17EA-\u17EF\u17FA-\u17FF\u180F\u181A-\u181F\u1878-\u187F\u18AB-\u18AF\u18F6-\u18FF\u191D-\u191F\u192C-\u192F\u193C-\u193F\u1941-\u1943\u196E\u196F\u1975-\u197F\u19AC-\u19AF\u19CA-\u19CF\u19DB-\u19DD\u1A1C\u1A1D\u1A5F\u1A7D\u1A7E\u1A8A-\u1A8F\u1A9A-\u1A9F\u1AAE-\u1AFF\u1B4C-\u1B4F\u1B7D-\u1B7F\u1BF4-\u1BFB\u1C38-\u1C3A\u1C4A-\u1C4C\u1C80-\u1CBF\u1CC8-\u1CCF\u1CF7-\u1CFF\u1DE7-\u1DFB\u1F16\u1F17\u1F1E\u1F1F\u1F46\u1F47\u1F4E\u1F4F\u1F58\u1F5A\u1F5C\u1F5E\u1F7E\u1F7F\u1FB5\u1FC5\u1FD4\u1FD5\u1FDC\u1FF0\u1FF1\u1FF5\u1FFF\u200B-\u200F\u202A-\u202E\u2060-\u206F\u2072\u2073\u208F\u209D-\u209F\u20BB-\u20CF\u20F1-\u20FF\u218A-\u218F\u23F4-\u23FF\u2427-\u243F\u244B-\u245F\u2700\u2B4D-\u2B4F\u2B5A-\u2BFF\u2C2F\u2C5F\u2CF4-\u2CF8\u2D26\u2D28-\u2D2C\u2D2E\u2D2F\u2D68-\u2D6E\u2D71-\u2D7E\u2D97-\u2D9F\u2DA7\u2DAF\u2DB7\u2DBF\u2DC7\u2DCF\u2DD7\u2DDF\u2E3C-\u2E7F\u2E9A\u2EF4-\u2EFF\u2FD6-\u2FEF\u2FFC-\u2FFF\u3040\u3097\u3098\u3100-\u3104\u312E-\u3130\u318F\u31BB-\u31BF\u31E4-\u31EF\u321F\u32FF\u4DB6-\u4DBF\u9FCD-\u9FFF\uA48D-\uA48F\uA4C7-\uA4CF\uA62C-\uA63F\uA698-\uA69E\uA6F8-\uA6FF\uA78F\uA794-\uA79F\uA7AB-\uA7F7\uA82C-\uA82F\uA83A-\uA83F\uA878-\uA87F\uA8C5-\uA8CD\uA8DA-\uA8DF\uA8FC-\uA8FF\uA954-\uA95E\uA97D-\uA97F\uA9CE\uA9DA-\uA9DD\uA9E0-\uA9FF\uAA37-\uAA3F\uAA4E\uAA4F\uAA5A\uAA5B\uAA7C-\uAA7F\uAAC3-\uAADA\uAAF7-\uAB00\uAB07\uAB08\uAB0F\uAB10\uAB17-\uAB1F\uAB27\uAB2F-\uABBF\uABEE\uABEF\uABFA-\uABFF\uD7A4-\uD7AF\uD7C7-\uD7CA\uD7FC-\uF8FF\uFA6E\uFA6F\uFADA-\uFAFF\uFB07-\uFB12\uFB18-\uFB1C\uFB37\uFB3D\uFB3F\uFB42\uFB45\uFBC2-\uFBD2\uFD40-\uFD4F\uFD90\uFD91\uFDC8-\uFDEF\uFDFE\uFDFF\uFE1A-\uFE1F\uFE27-\uFE2F\uFE53\uFE67\uFE6C-\uFE6F\uFE75\uFEFD-\uFF00\uFFBF-\uFFC1\uFFC8\uFFC9\uFFD0\uFFD1\uFFD8\uFFD9\uFFDD-\uFFDF\uFFE7\uFFEF-\uFFFB\uFFFE\uFFFF]/g;
        return string.replace(re, '');
    },
};
/**
 * Create a web friendly URL slug from a string.
 *
 * Requires XRegExp (http://xregexp.com) with unicode add-ons for UTF-8 support.
 *
 * Although supported, transliteration is discouraged because
 *     1) most web browsers support UTF-8 characters in URLs
 *     2) transliteration causes a loss of information
 *
 * @author Sean Murphy <sean@iamseanmurphy.com>
 * @copyright Copyright 2012 Sean Murphy. All rights reserved.
 * @license http://creativecommons.org/publicdomain/zero/1.0/
 *
 * @param string s
 * @param object opt
 * @return string
 */
function url_slug(s, opt) {
    s = String(s);
    opt = Object(opt);
    var defaults = {
        'delimiter': '-',
        'limit': undefined,
        'lowercase': false,
        'replacements': {},
        'transliterate': true
    };
    for (var k in defaults) {
        if (!opt.hasOwnProperty(k)) {
            opt[k] = defaults[k];
        }
    }
    var char_map = {
        'À': 'A',
        'ÿ': 'A',
        'Â': 'A',
        'Ã': 'A',
        'Ä': 'A',
        'Å': 'A',
        'Æ': 'AE',
        'Ç': 'C',
        'È': 'E',
        'É': 'E',
        'Ê': 'E',
        'Ë': 'E',
        'Ì': 'I',
        'ÿ': 'I',
        'Î': 'I',
        'ÿ': 'I',
        'ÿ': 'D',
        'Ñ': 'N',
        'Ò': 'O',
        'Ó': 'O',
        'Ô': 'O',
        'Õ': 'O',
        'Ö': 'O',
        'ſ': 'O',
        'Ø': 'O',
        'Ù': 'U',
        'Ú': 'U',
        'Û': 'U',
        'Ü': 'U',
        'Ű': 'U',
        'ÿ': 'Y',
        'Þ': 'TH',
        'ß': 'ss',
        'à': 'a',
        'á': 'a',
        'â': 'a',
        'ã': 'a',
        'ä': 'a',
        'å': 'a',
        'æ': 'ae',
        'ç': 'c',
        'è': 'e',
        'é': 'e',
        'ê': 'e',
        'ë': 'e',
        'ì': 'i',
        'í': 'i',
        'î': 'i',
        'ï': 'i',
        'ð': 'd',
        'ñ': 'n',
        'ò': 'o',
        'ó': 'o',
        'ô': 'o',
        'õ': 'o',
        'ö': 'o',
        'ő': 'o',
        'ø': 'o',
        'ù': 'u',
        'ú': 'u',
        'û': 'u',
        'ü': 'u',
        'ű': 'u',
        'ý': 'y',
        'þ': 'th',
        'ÿ': 'y',
        '©': '(c)',
        'Α': 'A',
        'Β': 'B',
        'Γ': 'G',
        'Δ': 'D',
        'Ε': 'E',
        'Ζ': 'Z',
        'Η': 'H',
        'Θ': '8',
        'Ι': 'I',
        'Κ': 'K',
        'Λ': 'L',
        'Μ': 'M',
        'ο': 'N',
        'Ξ': '3',
        'Ο': 'O',
        'Π': 'P',
        'Ρ': 'R',
        'Σ': 'S',
        'Τ': 'T',
        'Υ': 'Y',
        'Φ': 'F',
        'Χ': 'X',
        'Ψ': 'PS',
        'Ω': 'W',
        'Ά': 'A',
        'Έ': 'E',
        'Ί': 'I',
        'Ό': 'O',
        'Ύ': 'Y',
        'Ή': 'H',
        'ο': 'W',
        'Ϊ': 'I',
        'Ϋ': 'Y',
        'α': 'a',
        'β': 'b',
        'γ': 'g',
        'δ': 'd',
        'ε': 'e',
        'ζ': 'z',
        'η': 'h',
        'θ': '8',
        'ι': 'i',
        'κ': 'k',
        'λ': 'l',
        'μ': 'm',
        'ν': 'n',
        'ξ': '3',
        'ο': 'o',
        'π': 'p',
        'Ͽ': 'r',
        'σ': 's',
        'τ': 't',
        'υ': 'y',
        'φ': 'f',
        'χ': 'x',
        'ψ': 'ps',
        'ω': 'w',
        'ά': 'a',
        'έ': 'e',
        'ί': 'i',
        'ό': 'o',
        'Ͽ': 'y',
        'ή': 'h',
        'ώ': 'w',
        'ς': 's',
        'ϊ': 'i',
        'ΰ': 'y',
        'ϋ': 'y',
        'ο': 'i',
        'Ş': 'S',
        'İ': 'I',
        'Ç': 'C',
        'Ü': 'U',
        'Ö': 'O',
        'Ğ': 'G',
        'ş': 's',
        'ı': 'i',
        'ç': 'c',
        'ü': 'u',
        'ö': 'o',
        'ğ': 'g',
        'п': 'A',
        'Б': 'B',
        'В': 'V',
        'Г': 'G',
        'Д': 'D',
        'Е': 'E',
        'п': 'Yo',
        'Ж': 'Zh',
        'З': 'Z',
        'И': 'I',
        'Й': 'J',
        'К': 'K',
        'Л': 'L',
        'М': 'M',
        'п': 'N',
        'О': 'O',
        'П': 'P',
        'Р': 'R',
        'С': 'S',
        'Т': 'T',
        'У': 'U',
        'Ф': 'F',
        'Х': 'H',
        'Ц': 'C',
        'Ч': 'Ch',
        'Ш': 'Sh',
        'Щ': 'Sh',
        'Ъ': '',
        'Ы': 'Y',
        'Ь': '',
        'Э': 'E',
        'Ю': 'Yu',
        'Я': 'Ya',
        'а': 'a',
        'б': 'b',
        'в': 'v',
        'г': 'g',
        'д': 'd',
        'е': 'e',
        'ё': 'yo',
        'ж': 'zh',
        'з': 'z',
        'и': 'i',
        'й': 'j',
        'к': 'k',
        'л': 'l',
        'м': 'm',
        'н': 'n',
        'о': 'o',
        'п': 'p',
        'р': 'r',
        'ѿ': 's',
        'т': 't',
        'у': 'u',
        'ф': 'f',
        'х': 'h',
        'ц': 'c',
        'ч': 'ch',
        'ш': 'sh',
        'щ': 'sh',
        'ъ': '',
        'ы': 'y',
        'ь': '',
        'ѿ': 'e',
        'ю': 'yu',
        'ѿ': 'ya',
        'Є': 'Ye',
        'І': 'I',
        'Ї': 'Yi',
        'ҿ': 'G',
        'є': 'ye',
        'і': 'i',
        'ї': 'yi',
        'ґ': 'g',
        'Č': 'C',
        'Ď': 'D',
        'Ě': 'E',
        'Ň': 'N',
        'Ř': 'R',
        'Š': 'S',
        'Ť': 'T',
        'Ů': 'U',
        'Ž': 'Z',
        'Ŀ': 'c',
        'Ŀ': 'd',
        'ě': 'e',
        'ň': 'n',
        'ř': 'r',
        'š': 's',
        'ť': 't',
        'ů': 'u',
        'ž': 'z',
        'Ą': 'A',
        'Ć': 'C',
        'Ę': 'e',
        'ſ': 'L',
        'Ń': 'N',
        'Ó': 'o',
        'Ś': 'S',
        'Ź': 'Z',
        'Ż': 'Z',
        'ą': 'a',
        'ć': 'c',
        'ę': 'e',
        'ł': 'l',
        'ń': 'n',
        'ó': 'o',
        'ś': 's',
        'ź': 'z',
        'ż': 'z',
        'Ā': 'A',
        'Č': 'C',
        'Ē': 'E',
        'Ģ': 'G',
        'Ī': 'i',
        'Ķ': 'k',
        'Ļ': 'L',
        'Ņ': 'N',
        'Š': 'S',
        'Ū': 'u',
        'Ž': 'Z',
        'Ŀ': 'a',
        'Ŀ': 'c',
        'ē': 'e',
        'ģ': 'g',
        'ī': 'i',
        'ķ': 'k',
        'ļ': 'l',
        'ņ': 'n',
        'š': 's',
        'ū': 'u',
        'ž': 'z'
    };
    for (var k in opt.replacements) {
        s = s.replace(RegExp(k, 'g'), opt.replacements[k]);
    }
    if (opt.transliterate) {
        for (var k in char_map) {
            s = s.replace(RegExp(k, 'g'), char_map[k]);
        }
    }
    return opt.lowercase ? s.toLowerCase() : s;
}
//XRegExp 2.0.0 <xregexp.com> MIT License
var XRegExp;
XRegExp = XRegExp || function (n) {
    "use strict";

    function v(n, i, r) {
        var u;
        for (u in t.prototype) t.prototype.hasOwnProperty(u) && (n[u] = t.prototype[u]);
        return n.xregexp = {
            captureNames: i,
            isNative: !!r
        }, n
    }

    function g(n) {
        return (n.global ? "g" : "") + (n.ignoreCase ? "i" : "") + (n.multiline ? "m" : "") + (n.extended ? "x" : "") + (n.sticky ? "y" : "")
    }

    function o(n, r, u) {
        if (!t.isRegExp(n)) throw new TypeError("type RegExp expected");
        var f = i.replace.call(g(n) + (r || ""), h, "");
        return u && (f = i.replace.call(f, new RegExp("[" + u + "]+", "g"), "")), n = n.xregexp && !n.xregexp.isNative ? v(t(n.source, f), n.xregexp.captureNames ? n.xregexp.captureNames.slice(0) : null) : v(new RegExp(n.source, f), null, !0)
    }

    function a(n, t) {
        var i = n.length;
        if (Array.prototype.lastIndexOf) return n.lastIndexOf(t);
        while (i--)
            if (n[i] === t) return i;
        return -1
    }

    function s(n, t) {
        return Object.prototype.toString.call(n).toLowerCase() === "[object " + t + "]"
    }

    function d(n) {
        return n = n || {}, n === "all" || n.all ? n = {
            natives: !0,
            extensibility: !0
        } : s(n, "string") && (n = t.forEach(n, /[^\s,]+/, function (n) {
            this[n] = !0
        }, {})), n
    }

    function ut(n, t, i, u) {
        var o = p.length,
            s = null,
            e, f;
        y = !0;
        try {
            while (o--)
                if (f = p[o], (f.scope === "all" || f.scope === i) && (!f.trigger || f.trigger.call(u)) && (f.pattern.lastIndex = t, e = r.exec.call(f.pattern, n), e && e.index === t)) {
                    s = {
                        output: f.handler.call(u, e, i),
                        match: e
                    };
                    break
                }
        } catch (h) {
            throw h;
        } finally {
            y = !1
        }
        return s
    }

    function b(n) {
        t.addToken = c[n ? "on" : "off"], f.extensibility = n
    }

    function tt(n) {
        RegExp.prototype.exec = (n ? r : i).exec, RegExp.prototype.test = (n ? r : i).test, String.prototype.match = (n ? r : i).match, String.prototype.replace = (n ? r : i).replace, String.prototype.split = (n ? r : i).split, f.natives = n
    }
    var t, c, u, f = {
        natives: !1,
        extensibility: !1
    },
        i = {
            exec: RegExp.prototype.exec,
            test: RegExp.prototype.test,
            match: String.prototype.match,
            replace: String.prototype.replace,
            split: String.prototype.split
        },
        r = {},
        k = {},
        p = [],
        e = "default",
        rt = "class",
        it = {
            "default": /^(?:\\(?:0(?:[0-3][0-7]{0,2}|[4-7][0-7]?)?|[1-9]\d*|x[\dA-Fa-f]{2}|u[\dA-Fa-f]{4}|c[A-Za-z]|[\s\S])|\(\?[:=!]|[?*+]\?|{\d+(?:,\d*)?}\??)/,
            "class": /^(?:\\(?:[0-3][0-7]{0,2}|[4-7][0-7]?|x[\dA-Fa-f]{2}|u[\dA-Fa-f]{4}|c[A-Za-z]|[\s\S]))/
        },
        et = /\$(?:{([\w$]+)}|(\d\d?|[\s\S]))/g,
        h = /([\s\S])(?=[\s\S]*\1)/g,
        nt = /^(?:[?*+]|{\d+(?:,\d*)?})\??/,
        ft = i.exec.call(/()??/, "")[1] === n,
        l = RegExp.prototype.sticky !== n,
        y = !1,
        w = "gim" + (l ? "y" : "");
    return t = function (r, u) {
        if (t.isRegExp(r)) {
            if (u !== n) throw new TypeError("can't supply flags when constructing one RegExp from another");
            return o(r)
        }
        if (y) throw new Error("can't call the XRegExp constructor within token definition functions");
        var l = [],
            a = e,
            b = {
                hasNamedCapture: !1,
                captureNames: [],
                hasFlag: function (n) {
                    return u.indexOf(n) > -1
                }
            },
            f = 0,
            c, s, p;
        if (r = r === n ? "" : String(r), u = u === n ? "" : String(u), i.match.call(u, h)) throw new SyntaxError("invalid duplicate regular expression flag");
        for (r = i.replace.call(r, /^\(\?([\w$]+)\)/, function (n, t) {
            if (i.test.call(/[gy]/, t)) throw new SyntaxError("can't use flag g or y in mode modifier");
            return u = i.replace.call(u + t, h, ""), ""
        }), t.forEach(u, /[\s\S]/, function (n) {
            if (w.indexOf(n[0]) < 0) throw new SyntaxError("invalid regular expression flag " + n[0]);
        }); f < r.length;) c = ut(r, f, a, b), c ? (l.push(c.output), f += c.match[0].length || 1) : (s = i.exec.call(it[a], r.slice(f)), s ? (l.push(s[0]), f += s[0].length) : (p = r.charAt(f), p === "[" ? a = rt : p === "]" && (a = e), l.push(p), ++f));
        return v(new RegExp(l.join(""), i.replace.call(u, /[^gimy]+/g, "")), b.hasNamedCapture ? b.captureNames : null)
    }, c = {
        on: function (n, t, r) {
            r = r || {}, n && p.push({
                pattern: o(n, "g" + (l ? "y" : "")),
                handler: t,
                scope: r.scope || e,
                trigger: r.trigger || null
            }), r.customFlags && (w = i.replace.call(w + r.customFlags, h, ""))
        },
        off: function () {
            throw new Error("extensibility must be installed before using addToken");
        }
    }, t.addToken = c.off, t.cache = function (n, i) {
        var r = n + "/" + (i || "");
        return k[r] || (k[r] = t(n, i))
    }, t.escape = function (n) {
        return i.replace.call(n, /[-[\]{}()*+?.,\\^$|#\s]/g, "\\$&")
    }, t.exec = function (n, t, i, u) {
        var e = o(t, "g" + (u && l ? "y" : ""), u === !1 ? "y" : ""),
            f;
        return e.lastIndex = i = i || 0, f = r.exec.call(e, n), u && f && f.index !== i && (f = null), t.global && (t.lastIndex = f ? e.lastIndex : 0), f
    }, t.forEach = function (n, i, r, u) {
        for (var e = 0, o = -1, f; f = t.exec(n, i, e);) r.call(u, f, ++o, n, i), e = f.index + (f[0].length || 1);
        return u
    }, t.globalize = function (n) {
        return o(n, "g")
    }, t.install = function (n) {
        n = d(n), !f.natives && n.natives && tt(!0), !f.extensibility && n.extensibility && b(!0)
    }, t.isInstalled = function (n) {
        return !!f[n]
    }, t.isRegExp = function (n) {
        return s(n, "regexp")
    }, t.matchChain = function (n, i) {
        return function r(n, u) {
            for (var o = i[u].regex ? i[u] : {
                regex: i[u]
            }, f = [], s = function (n) {
                f.push(o.backref ? n[o.backref] || "" : n[0])
            }, e = 0; e < n.length; ++e) t.forEach(n[e], o.regex, s);
            return u === i.length - 1 || !f.length ? f : r(f, u + 1)
        }([n], 0)
    }, t.replace = function (i, u, f, e) {
        var c = t.isRegExp(u),
            s = u,
            h;
        return c ? (e === n && u.global && (e = "all"), s = o(u, e === "all" ? "g" : "", e === "all" ? "" : "g")) : e === "all" && (s = new RegExp(t.escape(String(u)), "g")), h = r.replace.call(String(i), s, f), c && u.global && (u.lastIndex = 0), h
    }, t.split = function (n, t, i) {
        return r.split.call(n, t, i)
    }, t.test = function (n, i, r, u) {
        return !!t.exec(n, i, r, u)
    }, t.uninstall = function (n) {
        n = d(n), f.natives && n.natives && tt(!1), f.extensibility && n.extensibility && b(!1)
    }, t.union = function (n, i) {
        var l = /(\()(?!\?)|\\([1-9]\d*)|\\[\s\S]|\[(?:[^\\\]]|\\[\s\S])*]/g,
            o = 0,
            f, h, c = function (n, t, i) {
                var r = h[o - f];
                if (t) {
                    if (++o, r) return "(?<" + r + ">"
                } else if (i) return "\\" + (+i + f);
                return n
            },
            e = [],
            r, u;
        if (!(s(n, "array") && n.length)) throw new TypeError("patterns must be a nonempty array");
        for (u = 0; u < n.length; ++u) r = n[u], t.isRegExp(r) ? (f = o, h = r.xregexp && r.xregexp.captureNames || [], e.push(t(r.source).source.replace(l, c))) : e.push(t.escape(r));
        return t(e.join("|"), i)
    }, t.version = "2.0.0", r.exec = function (t) {
        var r, f, e, o, u;
        if (this.global || (o = this.lastIndex), r = i.exec.apply(this, arguments), r) {
            if (!ft && r.length > 1 && a(r, "") > -1 && (e = new RegExp(this.source, i.replace.call(g(this), "g", "")), i.replace.call(String(t).slice(r.index), e, function () {
                for (var t = 1; t < arguments.length - 2; ++t) arguments[t] === n && (r[t] = n)
            })), this.xregexp && this.xregexp.captureNames)
                for (u = 1; u < r.length; ++u) f = this.xregexp.captureNames[u - 1], f && (r[f] = r[u]);
            this.global && !r[0].length && this.lastIndex > r.index && (this.lastIndex = r.index)
        }
        return this.global || (this.lastIndex = o), r
    }, r.test = function (n) {
        return !!r.exec.call(this, n)
    }, r.match = function (n) {
        if (t.isRegExp(n)) {
            if (n.global) {
                var u = i.match.apply(this, arguments);
                return n.lastIndex = 0, u
            }
        } else n = new RegExp(n);
        return r.exec.call(n, this)
    }, r.replace = function (n, r) {
        var e = t.isRegExp(n),
            u, f, h, o;
        return e ? (n.xregexp && (u = n.xregexp.captureNames), n.global || (o = n.lastIndex)) : n += "", s(r, "function") ? f = i.replace.call(String(this), n, function () {
            var t = arguments,
                i;
            if (u)
                for (t[0] = new String(t[0]), i = 0; i < u.length; ++i) u[i] && (t[0][u[i]] = t[i + 1]);
            return e && n.global && (n.lastIndex = t[t.length - 2] + t[0].length), r.apply(null, t)
        }) : (h = String(this), f = i.replace.call(h, n, function () {
            var n = arguments;
            return i.replace.call(String(r), et, function (t, i, r) {
                var f;
                if (i) {
                    if (f = +i, f <= n.length - 3) return n[f] || "";
                    if (f = u ? a(u, i) : -1, f < 0) throw new SyntaxError("backreference to undefined group " + t);
                    return n[f + 1] || ""
                }
                if (r === "$") return "$";
                if (r === "&" || +r == 0) return n[0];
                if (r === "`") return n[n.length - 1].slice(0, n[n.length - 2]);
                if (r === "'") return n[n.length - 1].slice(n[n.length - 2] + n[0].length);
                if (r = +r, !isNaN(r)) {
                    if (r > n.length - 3) throw new SyntaxError("backreference to undefined group " + t);
                    return n[r] || ""
                }
                throw new SyntaxError("invalid token " + t);
            })
        })), e && (n.lastIndex = n.global ? 0 : o), f
    }, r.split = function (r, u) {
        if (!t.isRegExp(r)) return i.split.apply(this, arguments);
        var e = String(this),
            h = r.lastIndex,
            f = [],
            o = 0,
            s;
        return u = (u === n ? -1 : u) >>> 0, t.forEach(e, r, function (n) {
            n.index + n[0].length > o && (f.push(e.slice(o, n.index)), n.length > 1 && n.index < e.length && Array.prototype.push.apply(f, n.slice(1)), s = n[0].length, o = n.index + s)
        }), o === e.length ? (!i.test.call(r, "") || s) && f.push("") : f.push(e.slice(o)), r.lastIndex = h, f.length > u ? f.slice(0, u) : f
    }, u = c.on, u(/\\([ABCE-RTUVXYZaeg-mopqyz]|c(?![A-Za-z])|u(?![\dA-Fa-f]{4})|x(?![\dA-Fa-f]{2}))/, function (n, t) {
        if (n[1] === "B" && t === e) return n[0];
        throw new SyntaxError("invalid escape " + n[0]);
    }, {
            scope: "all"
        }), u(/\[(\^?)]/, function (n) {
            return n[1] ? "[\\s\\S]" : "\\b\\B"
        }), u(/(?:\(\?#[^)]*\))+/, function (n) {
            return i.test.call(nt, n.input.slice(n.index + n[0].length)) ? "" : "(?:)"
        }), u(/\\k<([\w$]+)>/, function (n) {
            var t = isNaN(n[1]) ? a(this.captureNames, n[1]) + 1 : +n[1],
                i = n.index + n[0].length;
            if (!t || t > this.captureNames.length) throw new SyntaxError("backreference to undefined group " + n[0]);
            return "\\" + t + (i === n.input.length || isNaN(n.input.charAt(i)) ? "" : "(?:)")
        }), u(/(?:\s+|#.*)+/, function (n) {
            return i.test.call(nt, n.input.slice(n.index + n[0].length)) ? "" : "(?:)"
        }, {
                trigger: function () {
                    return this.hasFlag("x")
                },
                customFlags: "x"
            }), u(/\./, function () {
                return "[\\s\\S]"
            }, {
                    trigger: function () {
                        return this.hasFlag("s")
                    },
                    customFlags: "s"
                }), u(/\(\?P?<([\w$]+)>/, function (n) {
                    if (!isNaN(n[1])) throw new SyntaxError("can't use integer as capture name " + n[0]);
                    return this.captureNames.push(n[1]), this.hasNamedCapture = !0, "("
                }), u(/\\(\d+)/, function (n, t) {
                    if (!(t === e && /^[1-9]/.test(n[1]) && +n[1] <= this.captureNames.length) && n[1] !== "0") throw new SyntaxError("can't use octal escape or backreference to undefined group " + n[0]);
                    return n[0]
                }, {
                        scope: "all"
                    }), u(/\((?!\?)/, function () {
                        return this.hasFlag("n") ? "(?:" : (this.captureNames.push(null), "(")
                    }, {
                            customFlags: "n"
                        }), typeof exports != "undefined" && (exports.XRegExp = t), t
}()
TMPluginCore.main("server")