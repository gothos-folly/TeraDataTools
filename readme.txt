DataCenterUnpack:

* Decrypt and decompress a data center file given key and IV
* Extract the key/iv from a running Tera instance
* Outputs an info file which contains relevant information about a data center, including revision, key/iv, file name, file sizes and SHA-256 hashes.

GothosDC:

Replaces the older DCTools

* It doesn't require the user to hardcode section offsets and figures them out by itself. Simply point it at an unpacked data center.
* It parses the whole DataCenter in 5-10 seconds
* It preserves type information, instead of turning ints/bools/floats into strings that need to be parsed again.

DataTools:

Updated version, starting from coolyt's variant:

* It only contained the binary version of the Tera emulator classes, replaced that with source code taken from P5yl0's github.
* Replaced a few enums with strings, where it made sense. (e.g. NpcTitle)
* Replaced a bunch of ints with floats, where the data center uses floats
* Updated several of the parsers to work with current data, adding classes, skill-triggers, etc. and fixed handling a bunch of fields which are optional in recent versions

---

DataCenterUnpack and GothosDC are written by Gothos and released under the MIT license. DataTools was written by other people, I'm not sure which license applies to it.