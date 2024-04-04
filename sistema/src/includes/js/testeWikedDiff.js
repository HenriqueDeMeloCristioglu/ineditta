// @name        wikEd diff tool
// @version     1.0.0
// @date        September 25, 2014
// @description online tool for improved word-based diff library with block move detection
// @homepage    https://cacycle.altervista.org/wikEd-diff-tool.html
// @requires    https://en.wikipedia.org/w/index.php?title=User:Cacycle/diff.js&action=raw&ctype=text/javascript
// @author      Cacycle (https://en.wikipedia.org/wiki/User:Cacycle)
// @license     released into the public domain

// JSHint options: W004: is already defined, W100: character may get silently deleted
/* jshint -W004, -W100, newcap: true, browser: true, jquery: true, sub: true, bitwise: true, curly: true, evil: true, forin: true, freeze: true, globalstrict: true, immed: true, latedef: true, loopfunc: true, quotmark: single, strict: true, undef: true */
/* global console */

// turn on ECMAScript 5 strict mode
'use strict';

// define global objects
var WikEdDiffTool = {};
var WikEdDiff;
var wikEdDiffConfig;
var WED;


//
// WikEdDiffTool.init(): initialize
//

WikEdDiffTool.init = function() {

	// set debug shortcut
	if ( (WED === undefined) && (window.console !== undefined ) ) {
		WED = window.console.log;
	}

	// define config variable
	if (wikEdDiffConfig === undefined) {
		wikEdDiffConfig = {};
	}

	// define all wikEdDiff options
	WikEdDiffTool.options = [
		'fullDiff',
		'showBlockMoves',
		'charDiff',
		'repeatedDiff',
		'recursiveDiff',
		'recursionMax',
		'unlinkBlocks',
		'blockMinLength',
		'unlinkMax',
		'coloredBlocks',
		'debug',
		'timer',
		'unitTesting',
		'noUnicodeSymbols',
		'stripTrailingNewline'
	];

	// continue after content has loaded
	if (window.addEventListener !== undefined) {
		window.addEventListener('DOMContentLoaded', WikEdDiffTool.load);
	}
	else {
		window.onload = WikEdDiffTool.load;
	}
	return;
};


WikEdDiffTool.diff = function(velho, novo, block2) {

	
	wikEdDiffConfig.showBlockMoves = false;
	wikEdDiffConfig.fullDiff = false;
	wikEdDiffConfig.repeatedDiff = false;
	wikEdDiffConfig.recursiveDiff = false;

	var right = document.createElement("p")

	// console.log(velho);
	// console.log(novo);

	if (!velho || !novo) {
		right.innerHTML = !novo ? "Não existem cláusulas cadastradas na base de dados correspondentes ao ano selecionado!" : novo;
		block2.appendChild(right);
		block2.className = 'col-lg-6 nadaDeNovo'
		return
	}

	var oldString = velho;
	var newString = novo;
	var wikEdDiff = new WikEdDiff();
	var diffHtml = wikEdDiff.diff(oldString, newString);
	right.innerHTML = diffHtml;

	block2.appendChild(right);

	return;
};

WikEdDiffTool.diff2 = function(velho, novo, block1) {


	var left = document.createElement("p")

	if (!velho || !novo) {
		left.innerHTML = !velho ? "Não existem cláusulas cadastradas na base de dados correspondentes ao ano selecionado!" : velho;
		block1.appendChild(left)
		block1.className = 'col-lg-6 nadaDeNovo'
		return
	}

	var oldString = novo;
	var newString = velho;
	var wikEdDiff2 = new WikEdDiff2();
	var diffHtml = wikEdDiff2.diff(oldString, newString);
	left.innerHTML = diffHtml;

	block1.appendChild(left)

	return;
};


WikEdDiffTool.clear = function() {

	document.getElementById('old').value = '';
	document.getElementById('new').value = '';
	WikEdDiffTool.diff();
	return;
};


WikEdDiffTool.dropHandler = function( event ) {

	event.stopPropagation();
	event.preventDefault();

	// get FileList object.
	var fileListObj = event.dataTransfer.files;
	event.target.value = '';

	// get text from dropped files
	WikEdDiffTool.getFileText( fileListObj, event.target, 0 )
	return;
};

//
// WikEdDiffTool.getFileText(): get text file content, cycles through all files in file list object
//

WikEdDiffTool.getFileText = function( fileListObj, target, fileNumber ) {

	if ( fileNumber >= fileListObj.length ) {
		return;
	}
	var fileObj = fileListObj[ fileNumber ];
	if ( target.value !== '' ) {
		target.value += '\n\n'
	}

	// get size and format
	var size = fileObj.size;
	var sizeFormatted = size + '';
	sizeFormatted = sizeFormatted.replace( /(\d\d\d)?(\d\d\d)?(\d\d\d)?(\d\d\d)$/, ',$1,$2,$3,$4' );
	sizeFormatted = sizeFormatted.replace( /^,+/, '' );
	sizeFormatted = sizeFormatted.replace( /,,+/, ',' );
	target.value += encodeURI( fileObj.name ) + ' (' + sizeFormatted + ' bytes):\n';

	// check file length
	var contentMB = parseInt( size / 1024 / 1024 * 10 ) / 10;
	if ( contentMB > 10 ) {
		target.value += 'Error: file larger than 10 MB (' + contentMB + ' MB)\n';
		WikEdDiffTool.getFileText( fileListObj, target, fileNumber + 1 );
		return;
	}

	// read file content asynchronously
	var readerObj = new FileReader();
	readerObj.onload = function() {
		target.value += readerObj.result;
		WikEdDiffTool.getFileText( fileListObj, target, fileNumber + 1 );
		return;
	}
	readerObj.readAsText( fileObj );
	return;
}

//
// WikEdDiffTool.dragHandler(): event handler for dropping files on old or new fields
//

WikEdDiffTool.dragHandler = function( event ) {

	event.stopPropagation();
	event.preventDefault();
	event.dataTransfer.dropEffect = 'copy';
	return;
};

//
// WikEdDiffTool.preventDropHandler(): disable drag and drop over certain elements
//

WikEdDiffTool.preventDropHandler = function( event ) {

	event.stopPropagation();
	event.preventDefault();
	event.dataTransfer.dropEffect = 'none';
	return;
};

// initialize WikEdDiffTool
WikEdDiffTool.init();
