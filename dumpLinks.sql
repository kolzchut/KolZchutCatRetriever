-- Plain version
SELECT page_title FROM `page` WHERE page_namespace=0 AND page_is_redirect=0


-- Version that automatically adds the namespace to the title (add you
-- own custom namespaces if you have them):
SELECT 
	CONCAT(
		CASE page_namespace
			-- MediaWiki standard (canonical) namespaces
			WHEN 0 THEN ''
			WHEN 1 THEN 'Talk:'
			WHEN 2 THEN 'User:'
			WHEN 4 THEN 'Project:'
			WHEN 6 THEN 'File:'
			WHEN 8 THEN 'Mediawiki:'
			WHEN 10 THEN 'Template:'
			WHEN 12 THEN 'Help:'
			WHEN 14 THEN 'Category:'

			-- MediaWiki standard extension
			WHEN 274 THEN 'Widget:'

			-- Kol-Zchut cusom namespaces:
			WHEN 110 THEN 'אודות:'
			WHEN 112 THEN 'קהילת_ידע:'
			WHEN 116 THEN 'חדש:'
			WHEN 118 THEN 'הקפאה:'
			WHEN 120 THEN 'תרגול:'
			WHEN 122 THEN 'נתון:'
			
			-- Default so we can see if the above isn't comprehensive enough:
			ELSE CONCAT(page_namespace,':')
		END,
	page_title) AS title
FROM `page`
WHERE page_is_redirect=0

