@echo off
set output=.\release
set source=.
set variant=Debug
rmdir /Q /S "%output%"
md "%output%
copy "%source%\DataTools\bin\%variant%\*" "%output%\"
copy "%source%\DataCenterUnpack\bin\%variant%\*" "%output%\"
rem copy "%source%\ReadmeUser.txt" "%output%\readme.txt"
rem xcopy /E "%source%\resources" "%output%\resources\"
del "%output%\*.vshost*"
