let KBHandler = {
	_selword:null,
	_selwordid:null,
	
	init:function(){
		_selwordid = 0;
	},
	setWord:function(selectedWord)
	{
		_selword = selectedWord;
	},
	getWord:function()
	{
		let url = new URL(window.location.href);
		this.setWord(url.searchParams.get("word"));
		if (_selword!=null)
		{
			//_selwordid = 0;
			for (var i = 0; i < theObject.wordlist.length; i++)
			{
				if (theObject.wordlist[i].Occurence == _selword) { _selwordid=i; }
				if (_selwordid != null) 
				{ 
					this.setBaseSettings(); 
					this.parseWord(); 
				}
			}
			
		} else {
			
		}
	},
	setBaseSettings:function()
	{
		//highlight color
		let colorcode = 'rgb('+theObject.globrgb+")";
		$(".highlight").css('color', colorcode);
		$(".marker-color").css('color', colorcode);
		$("p.Tab_Wuert").css('color', colorcode);
		$("div.divTableBody").css('background-color', colorcode);
		$(".flex-child").css('border-color', colorcode);
		
		//audio
		let audiopath = 'audio/'+theObject.wordlist[_selwordid].MP3;
		$("#audio").attr('src',audiopath);
		$("#audiosource").attr('src',audiopath);
	},
	parseWord:function()
	{
		$("#word_LU").text(theObject.wordlist[_selwordid].LU);
		/* Pluriel test */
		$("#word_LUs").text(theObject.wordlist[_selwordid].LUs);
		
		$("#word_form").text(theObject.wordlist[_selwordid].WuertForm);
		
		$("#word_DE").text(theObject.wordlist[_selwordid].DE);
		$("#word_FR").text(theObject.wordlist[_selwordid].FR);
		$("#word_PT").text(theObject.wordlist[_selwordid].PT);
		$("#word_EN").text(theObject.wordlist[_selwordid].EN);
	}
};

KBHandler.init();
KBHandler.getWord();

let InfoShower = {
	log:function(str)
	{
		$("<div/>", {id:'info', style:'', text:str}).appendTo("body");
		return true;
	}
}

/*
*	Audio Loader & Player
*/
document.getElementById("audio").load();

function play() {
   var audio = document.getElementById("audio");
	audio.currentTime = 0;
	audio.play();
}

document.getElementById("audiocontainer").addEventListener("click", play);

/*
*	Helper functions
*/