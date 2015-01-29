
Create Table TemplateType
(
Code varchar(10) not null,
ListType varchar(10) not null,
TemplateName varchar(50)
CONSTRAINT pk_TemplateType_Code_ListType PRIMARY KEY (Code, ListType)
CONSTRAINT fk_TemplateType_Code FOREIGN KEY (Code)
REFERENCES RefTable (Code)
)


ALTER TABLE Listing
ADD TemplateName varchar(50)

ALTER TABLE RefCategory
ADD TemplateType varchar(10)


EXEC sp_RENAME 'Listing.ListingTypeCode' , 'ListType', 'COLUMN'