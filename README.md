Kol-Zchut Category Retriever (KZ-CatRetriever)
==============================================

This command line utility retrieves the *non-hidden* categories for articles.
While this can be theoratically achieved by an SQL query or the API,
in practice, unfortunately, MediaWiki does not store the categories
in the order they are present in the article.

## How it works
The utility currently works by receiving a CSV of links and
HTML-scraping each one, coupled with a regular expression
to retrieve the categories themselves.

, it is a very fragile
creature indeed.

## Caveats

- Because of the HTML-scraping and RegExp, this utility is a
  very fragile creature indeed. It might fail under skins
  other than Vector or several MediaWiki versions.

- Apart from being fragile and prone to break, this utility is
  also server-resources unfriendly and slow; do not run it on a
  wiki you do not own or have been given permission to do so.
  Some may even block you as an attacker, as you will be hitting
  the server thousands of time in succession.

## How to use

### Get a list of pages
To get the list of links, you need to do the following:

1. Get a list of pages from the database; this can be done by
   running the included dumpLinks.sql, which currently only
   dumps the titles of articles in the Main namespace that
   are not redirects; you can change it as needed.
2. Save the result to a CSV file.
3. Duplicate the column; in the new column, url_encode the
   page names (it might be that exchange every quote mark
   with "%22" suffice).
4. Add the fully qualified article path for you wiki in front
   of the encoded page names. For example, if you have an
   article named "How to run", then you would now have URLs
   such as http://www.example.com/wiki/How_to_run.

### Run the utility
Now you can run the utility by issuing the command:
KolZchutCatRetriever PageList.csv
(in linux this must be prefixed by "mono ")

You may also supply the name of the output file as a second category.

### Wait

The utility will now run for a long time (could be an hour
for every 5,000 pages, but it greatly depends on the server).
At the end of that time, you will have a CSV file with the same
two columns as the original, plus a column for each article category,
in the same order they show in the wiki.

### Success?
Hopefully it worked fine for you. If not, you can try to fix it
yourself by forking the code, or open an issue.
