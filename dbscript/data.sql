-- TemplateType
INSERT INTO [dbo].[TemplateType]([Code],[ListType],[TemplateName])
VALUES ('T1','S','_residential')

INSERT INTO [dbo].[TemplateType]([Code],[ListType],[TemplateName])
VALUES ('T1','R','_residentialRent')

INSERT INTO [dbo].[TemplateType]([Code],[ListType],[TemplateName])
VALUES ('T2','S','_commercial')

INSERT INTO [dbo].[TemplateType]([Code],[ListType],[TemplateName])
VALUES ('T2','R','_commercialRent')

INSERT INTO [dbo].[TemplateType]([Code],[ListType],[TemplateName])
VALUES ('T3','S','_car')

INSERT INTO [dbo].[TemplateType]([Code],[ListType],[TemplateName])
VALUES ('T3','R','_carRent')

INSERT INTO [dbo].[TemplateType]([Code],[ListType],[TemplateName])
VALUES ('T5','S','_generic')

INSERT INTO [dbo].[TemplateType]([Code],[ListType],[TemplateName])
VALUES ('T5','W','_generic')

-- RefTable
INSERT INTO RefTable (Code, name, Description, Type, isActive) 
VALUES('T1', 'Residential Template', 'Residential Template', 'TEMPLATE', 1)

INSERT INTO RefTable (Code, name, Description, Type, isActive)
VALUES('T2', 'Commercial Template', 'Commercial Template', 'TEMPLATE', 1)

INSERT INTO RefTable (Code, name, Description, Type, isActive)
VALUES('T3', 'Car Template', 'Car Template', 'TEMPLATE', 1)

INSERT INTO RefTable (Code, name, Description, Type, isActive)
VALUES('T4', 'Item Template', 'Item Template', 'TEMPLATE', 1)

INSERT INTO RefTable (Code, name, Description, Type, isActive)
VALUES('T5', 'Generic Template', 'Generic Template', 'TEMPLATE', 1)