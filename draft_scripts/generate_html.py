import os
import xlrd

path_to_excel_file = R"<put path to the excel files with results>"

if not os.path.isfile(path_to_excel_file):
	print("Please specify path to excel file (inside script). Or add arg parse here :)")
	exit(0)

# Read excel file with results
results = dict()
header = []

rb = xlrd.open_workbook(path_to_excel_file)
sheet = rb.sheet_by_index(0)
header = sheet.row_values(0)
for rownum in range(1, sheet.nrows):
	row = sheet.row_values(rownum)
	results[os.path.splitext(row[0])[0]] = row

for key in results:
	data = results[key]
	formatted_data = ""
	for index in range(len(header)):
		data_part = data[index]
		
		if "Valid" in header[index]:
			data_part = "True" if data_part else "False"
			print(data_part)
		elif type(data_part) is float:
			data_part = int(data_part)

		formatted_data += '<b>' + header[index] + '</b>: <span style="float: right;">' + str(data_part) + "</span><br>"
	results[key] = formatted_data

html_prefix = '<!DOCTYPE html>\
<html>\
<head>\
<style>\
html, body {\
    height: 100%;\
}\
.image-cell {\
	height: 100%;\
	width: 60%;\
	border: 1px solid black;\
}\
p.attribs {\
	text-align: left;\
	font-size: 125%;\
	line-height: 140%;\
	margin-left: 10px;\
}\
</style>\
</head>\
<body>\
\
<h1 style="text-align: center;">Test results for Mesh Recovery with different dimensions</h1>'

htmp_suffix = '</body> </html>'

images = os.listdir(r"test_results")
images_tags = ""

for image in images:
	name = os.path.splitext(image)[0]
	images_tags += '<div style="text-align: center; margin-top: 30px;">\
	<h3>' + name + '</h3>\
	<table>\
	<tr>\
	<td class="image-cell"><img style="display:block;" width="100%" height="100%" src="test_results/' + str(image) + '" alt="' + name + '"></td>\
	<td valign="top"><p class="attribs">' + results[name] + '</p></td>\
	</tr>\
	</table>\
	<br>\n'

with open("results.html", 'w') as file:
	file.write(html_prefix + images_tags + htmp_suffix)
