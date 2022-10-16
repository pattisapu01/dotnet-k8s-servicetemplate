CREATE TABLE IF NOT EXISTS $ext_safeprojectname$(
 		id int not null,
        name character varying(200) NOT NULL,
        description character varying(40) NOT NULL,
        quantity int not null,
        amount double precision NOT NULL,
        isactive boolean          
    );

    ALTER TABLE ONLY $ext_safeprojectname$
        ADD CONSTRAINT $ext_safeprojectname$_pkey PRIMARY KEY(id);      

       CREATE SEQUENCE $ext_safeprojectname$_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER SEQUENCE $ext_safeprojectname$_id_seq OWNED BY $ext_safeprojectname$.id;
       
ALTER TABLE ONLY $ext_safeprojectname$ ALTER COLUMN id SET DEFAULT nextval('$ext_safeprojectname$_id_seq'::regclass);

